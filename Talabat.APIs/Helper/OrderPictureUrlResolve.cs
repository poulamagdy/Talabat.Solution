using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.APIs.Helper
{
	public class OrderPictureUrlResolve : IValueResolver<OrderItem, OrderItemDTO, string>
	{
		private readonly IConfiguration _configuration;

		public OrderPictureUrlResolve(IConfiguration configuration)
        {
			_configuration = configuration;
		}
        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.Product.PictureUrl))
				return $"{_configuration["ApiBaseUrl"]}{source.Product.PictureUrl}";

			return string.Empty;
		}
	}
}
