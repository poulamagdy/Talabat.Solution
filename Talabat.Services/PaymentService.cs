using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specification.Order_Spec;

namespace Talabat.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration,
			IBasketRepository basketRepository,
			IUnitOfWork unitOfWork)
        {
			_configuration = configuration;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId)
		{
			// Secret Key

			StripeConfiguration.ApiKey = _configuration["StripeKey:Secretkey"];

			// Get Basket

			var basket = await _basketRepository.GetBasketAsync(basketId);

			if (basket is null) return null;
			// total = subtotal + dm.cost

			var shippingPrice = 0m;

			if (basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);
				shippingPrice = deliveryMethod.Cost;
			}

			if (basket.Items.Count > 0)
			{
				foreach (var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Core.Entities.Product, int>().GetByIdAsync(item.Id);
					if (item.Price != product.Price)
						item.Price = product.Price;
				}
			}

			var subTotal = basket.Items.Sum(item => item.Price * item.Quantity);

			// create PaymentIntent

			var service = new PaymentIntentService();

			PaymentIntent paymentIntent;

			if (string.IsNullOrEmpty(basket.PaymentIntentId)) // create
			{
				var option = new PaymentIntentCreateOptions()
				{
					Amount = (long) subTotal * 100 + (long) shippingPrice * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() {"card"}
				};

				paymentIntent = await service.CreateAsync(option);

				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else // update
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)subTotal * 100 + (long)shippingPrice * 100,
				};

				paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);

				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}

			await _basketRepository.UpdateOrCreateBasketAsync(basket);

			return basket;
		}

		public async Task<Order> UpdatePaymentIntentSecceedOrFailed(string paymentIntentId, bool flag)
		{
			var spec = new OrderWithPaymentSpec(paymentIntentId);
			var order = await _unitOfWork.Repository<Order, int>().GetByEntityAsyncWithSpec(spec);

			if (flag)
				order.Status = OrderStatus.PaymentReceived;
			else
				order.Status = OrderStatus.PaymentFailed;

			_unitOfWork.Repository<Order, int>().Update(order);
			await _unitOfWork.CompleteAsync();
			return order;
		}
	}
}
