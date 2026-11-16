using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Repository.Data

{
	public class StoreContextSeedData
	{
		public static async Task SeedDataAsync(StoreContext context , ILoggerFactory loggerFactory)
		{
			try
			{
				if (!context.Set<ProductBrand>().Any())
				{
					var brandData = File.ReadAllText("../Talabat.Repository/DbContexts/SeedData/brands.json");

					var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);

					if (brands is not null && brands.Count > 0)
					{
						foreach (var brand in brands)
							await context.Set<ProductBrand>().AddAsync(brand);
					}
				}

				if (!context.Set<ProductType>().Any())
				{
					var type = File.ReadAllText("../Talabat.Repository/DbContexts/SeedData/types.json");

					var types = JsonSerializer.Deserialize<List<ProductType>>(type);

					if (types is not null && types.Count > 0)
					{
						foreach (var _type in types)
							await context.Set<ProductType>().AddAsync(_type);
					}
				}

				if (!context.Set<Product>().Any())
				{
					var productData = File.ReadAllText("../Talabat.Repository/DbContexts/SeedData/products.json");

					var products = JsonSerializer.Deserialize<List<Product>>(productData);

					if (products is not null && products.Count > 0)
					{
						foreach (var product in products)
							await context.Set<Product>().AddAsync(product);
					}
				}

				if (!context.Set<DeliveryMethod>().Any())
				{
					var deliveryMethodData = File.ReadAllText("../Talabat.Repository/DbContexts/SeedData/delivery.json");

					var deliverys = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodData);

					if (deliverys is not null && deliverys.Count > 0)
					{
						foreach (var delivery in deliverys)
							await context.Set<DeliveryMethod>().AddAsync(delivery);
					}
				}

				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<StoreContextSeedData>();
				logger.LogError(ex.Message);
			}
		}
    }
}
