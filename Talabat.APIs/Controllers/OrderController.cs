using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregation;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class OrderController : BaseController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrderController(IOrderService orderService, IMapper mapper)
        {
			_orderService = orderService;
			_mapper = mapper;
		}
		[Authorize]
        [HttpPost]
		[ProducesResponseType(typeof(OrderReturnDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<OrderReturnDTO>> CreateOrder(OrderDTO inputOrder)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

			var mapperAddress = _mapper.Map<Address>(inputOrder.ShippingAddress);

			var order = await _orderService.CreateOrderAsync(buyerEmail, inputOrder.BasketId, inputOrder.DeliveryMethodId, mapperAddress);

			if (order is null) return BadRequest(new ApiResponse(400, "Error While Creating Your Order"));

			var OrderDto = _mapper.Map<OrderReturnDTO>(order);

			return Ok(OrderDto);
		}

		[HttpGet]
		[Authorize]
		[ProducesResponseType(typeof(IReadOnlyList<OrderReturnDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IReadOnlyList<OrderReturnDTO>>> GetOrderForUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			var orders = await _orderService.GetOrdersForSpecificUserAsync(email);

			if (orders is null) return NotFound(new ApiResponse(404, "There is no Order For This User"));

			var OrderDto = _mapper.Map<OrderReturnDTO>(orders);

			return Ok(OrderDto);
		}

		[HttpGet("{id}")]
		[Authorize]
		[ProducesResponseType(typeof(OrderReturnDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<OrderReturnDTO>> GetOrderForUserById(int id)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

			var order = await _orderService.GetOrdersByIdForSpecificUserAsync(buyerEmail, id);

			if (order is null) return NotFound(new ApiResponse(404, $"There is no Order with id = {id} For This User"));

			var OrderDto = _mapper.Map<OrderReturnDTO>(order);

			return Ok(OrderDto);
		}
		[HttpGet("DeliveryMethod")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
		{
			var deliveryMrthods = await _orderService.GetDeliveryMethodAsync();

			return Ok(deliveryMrthods);
		}
	}
}
