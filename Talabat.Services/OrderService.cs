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
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			//1.Get Basket From Basket Repo

			var basket = await _basketRepository.GetBasketAsync(basketId);

			//2.Get Selected Items at Basket From Product Repo

			var orderItems = new List<OrderItem>();

			if(basket?.Items.Count > 0)
			{
				foreach (var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Product, int>().GetByIdAsync(item.Id);
					var productOrder = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productOrder, product.Price, item.Quantity);
					orderItems.Add(orderItem);
				}
			}

			//3.Calculate SubTotal

			var subTotal = orderItems.Sum(o => o.Quantity * o.Price);

			//4.Get Delivery Method From DeliveryMethod Repo

			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(deliveryMethodId);

			//5.Create Order

			var spec = new OrderWithPaymentSpec(basket.PaymentIntentId);

			var exOeder = await _unitOfWork.Repository<Order, int>().GetByEntityAsyncWithSpec(spec);

			if (exOeder is not null) 
			{
				_unitOfWork.Repository<Order, int>().Delete(exOeder);
				await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
			}

			var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

			//6.Add Order Locally

			await _unitOfWork.Repository<Order, int>().AddAsync(order);

			//7.Save Order To Database[ToDo]
			var result = await _unitOfWork.CompleteAsync();

			if (result <= 0) return null;
			
			return order;
		}

		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
			=> await _unitOfWork.Repository<DeliveryMethod, int>().GetAllAsync();

		public Task<Order> GetOrdersByIdForSpecificUserAsync(string buyerEmail, int orderId)
		{
			var spec = new OrderSpecification(buyerEmail, orderId);

			var order = _unitOfWork.Repository<Order, int>().GetByEntityAsyncWithSpec(spec);

			return order;
		}

		public  async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
		{
			var orderSpec = new OrderSpecification(buyerEmail);

			var orders = await _unitOfWork.Repository<Order, int>().GetAllAsyncWithSpec(orderSpec);

			return orders;
		}
	}
}
