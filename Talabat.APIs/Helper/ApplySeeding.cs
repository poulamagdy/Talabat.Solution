using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Identity;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs.Helper
{
	public static class ApplySeeding
	{
		public static async Task ApplySeedingAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var services = scope.ServiceProvider;

			var loogerFactory = services.GetRequiredService<ILoggerFactory>();

			try
			{
				var dbContext = services.GetRequiredService<StoreContext>();

				await dbContext.Database.MigrateAsync();

				var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();

				await identityDbContext.Database.MigrateAsync();

				await StoreContextSeedData.SeedDataAsync(dbContext, loogerFactory);

				var userManager = services.GetRequiredService<UserManager<AppUser>>();

				await AppIdentityDbContextSeed.SeedUserAsync(userManager);
			}
			catch (Exception ex)
			{

				var logger = loogerFactory.CreateLogger<Program>();

				logger.LogError(ex, "an error occured during apply the migration");
			}
		}
	}
}
