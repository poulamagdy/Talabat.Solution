using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helper;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Services;

namespace Talabat.APIs.Extantions
{
	public static class ApplicationServicesExtantion
	{
		public static IServiceCollection AddAplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			//services.AddScoped<IGenericRepository<Product, int>, GenericRepository<Product, int>>();
			//services.AddScoped<IGenericRepository<ProductBrand, int>, GenericRepository<ProductBrand, int>>();
			//services.AddScoped<IGenericRepository<ProductType, int>, GenericRepository<ProductType, int>>();
			services.AddScoped<IBasketRepository, BasketRepository>();

			services.AddAutoMapper(typeof(MappingProfile));

			services.Configure<ApiBehaviorOptions>(option =>
			{
				option.InvalidModelStateResponseFactory = actioncontext =>
				{
					var error = actioncontext.ModelState
								.Where(model => model.Value.Errors.Count > 0)
								.SelectMany(model => model.Value.Errors)
								.Select(model => model.ErrorMessage)
								.ToArray();

					var errorResponse = new ApiValidation
					{
						Errors = error
					};

					return new BadRequestObjectResult(errorResponse);
				};
			});

			return services;
		}
	}
}
