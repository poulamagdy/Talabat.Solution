using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{	
	public class PaymentController : BaseController
	{
		private readonly IPaymentService _paymentService;
		private readonly IMapper _mapper;
		const string endpointSecret = "whsec_f94f776e750a840166dc833c9298fce5301cb3fb02cc8d040dc8dc03dd8ce24f";
		public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
			_paymentService = paymentService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(CustomerBasketDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost]
		[Authorize]
		public async Task<ActionResult<CustomerBasketDTO>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var customer = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);

			if (customer is null) return BadRequest(new ApiResponse(400));

			var customerDto = _mapper.Map<CustomerBasketDTO>(customer);

			return Ok(customerDto);
		}

		[HttpPost("webhook")]
		public async Task<ActionResult> StripeWebhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

				// Handle the event
				if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{
					await _paymentService.UpdatePaymentIntentSecceedOrFailed(paymentIntent.Id, false);
				}
				else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{
					await _paymentService.UpdatePaymentIntentSecceedOrFailed(paymentIntent.Id, true);

				}
				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		}
	}
}
