using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
	public class BasketController : BaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepository, IMapper mapper)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CustomerBasketDTO>> GetBasket(string id)
		{
			var basket = await _basketRepository.GetBasketAsync(id);

			if (basket is null) return new CustomerBasketDTO(id);

			var customerBasketDTO = _mapper.Map<CustomerBasketDTO>(basket);

			return Ok(customerBasketDTO);
		}

		[HttpPost]
		public async Task<ActionResult<CustomerBasketDTO>> UpdateBasket(CustomerBasketDTO basketDTO)
		{
			var mapperBasket = _mapper.Map<CustomerBasket>(basketDTO);

			var basket = await _basketRepository.UpdateOrCreateBasketAsync(mapperBasket);

			if (basket is null) return BadRequest(new ApiResponse(400));

			var customerBasketDto = _mapper.Map<CustomerBasketDTO>(basket);

			return Ok(customerBasketDto);
		}

		[HttpDelete]
		public async Task<ActionResult<bool>> Delete(string id) 
			=> await _basketRepository.DeleteBasketAsync(id);
    }
}
