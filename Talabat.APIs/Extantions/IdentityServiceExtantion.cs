using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Repository.Identity;
using Talabat.Services;

namespace Talabat.APIs.Extantions
{
    public static class IdentityServiceExtantion
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration) 
		{
			services.AddScoped<ITokenServices, TokenServices>();

			services.AddIdentity<AppUser, IdentityRole>()
							.AddEntityFrameworkStores<AppIdentityDbContext>();

			services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
					.AddJwtBearer(option =>
					{
						option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
						{
							ValidateIssuer = true,
							ValidIssuer = configuration["JWT:Issuer"],
							ValidateAudience = true,
							ValidAudience = configuration["JWT:Audience"],
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
						};
					});

			return services;
		}
	}
}
