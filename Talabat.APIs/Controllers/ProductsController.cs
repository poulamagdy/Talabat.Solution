using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helper;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specification;

namespace Talabat.APIs.Controllers
{
	[Authorize]
	public class ProductsController : BaseController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductsController(IUnitOfWork unitOfWork
			,IMapper mapper)
        {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
        [HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetAllProduct([FromQuery]ProductSpecParams specParams)
		{
			var spec = new ProductWithProductBrandAndTypeSpec(specParams); 

			var products = await _unitOfWork.Repository<Product, int>().GetAllAsyncWithSpec(spec);

			var mapperProduct = _mapper.Map<IReadOnlyList<ProductToReturnDTO>>(products);

			var countSpec = new ProductWithFiltrationForCountAsync(specParams);

			var count = await _unitOfWork.Repository<Product, int>().GetCountWithSpecAsync(countSpec);

			var pagination = new Pagination<ProductToReturnDTO>()
			{
				PageSize = specParams.PageSize,
				PageIndex = specParams.PageIndex,
				Count = count,
				Data = mapperProduct
			};

			return Ok(pagination);
		}
		[ProducesResponseType(typeof(ProductToReturnDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDTO>> GetById(int id)
		{
			if (id == null) return BadRequest(new ApiResponse(400));

			var spec = new ProductWithProductBrandAndTypeSpec(id);

			var product = await _unitOfWork.Repository<Product, int>().GetByEntityAsyncWithSpec(spec);

			if (product is null) return NotFound(new ApiResponse(404));

			var mapperProduct = _mapper.Map<ProductToReturnDTO>(product);

			return Ok(mapperProduct);
		}
		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrandDTO>>> GetAllBrands()
		{
			var brands = await _unitOfWork.Repository<ProductBrand, int>().GetAllAsync();

			var mapperBrands = _mapper.Map<IReadOnlyList<ProductBrandDTO>>(brands);

			return Ok(mapperBrands);
		}

		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductTypeDTO>>> GetAllTypes()
		{
			var types = await _unitOfWork.Repository<ProductType, int>().GetAllAsync();

			var mapperTypes = _mapper.Map<IReadOnlyList<ProductTypeDTO>>(types);

			return Ok(mapperTypes);
		}
	}
}
