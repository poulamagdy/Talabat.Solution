using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AppUser> userManager)
		{
			if (!userManager.Users.Any())
			{
				var user = new AppUser()
				{
					DisplayName = "Poula Magdy",
					Email = "poulamagdy852@gmail.com",
					UserName = "poulamagdy852",
					PhoneNumber = "01551721330"
				};

				await userManager.CreateAsync(user, "P@ssw0rd");
			}
		}
	}
}
