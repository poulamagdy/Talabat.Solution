using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.Services
{
    public class TokenServices : ITokenServices
	{
		private readonly IConfiguration _configuration;

		public TokenServices(IConfiguration configuration)
        {
			_configuration = configuration;
		}
        public async Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager)
		{

			//private claims
			var authClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.GivenName, appUser.DisplayName),
				new Claim(ClaimTypes.Email, appUser.Email)
			};

			var roles = await userManager.GetRolesAsync(appUser);

			foreach (var role in roles) 
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			//key
			var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

			// Create Token
			var token = new JwtSecurityToken(
				//registerClaims
				issuer : _configuration["JWT:Issuer"],
				audience : _configuration["JWT:Audience"],
				expires : DateTime.Now.AddDays(double.Parse(_configuration["JWT:expirationInDay"])),

				//privateClaims
				claims: authClaims,

				// Header
				signingCredentials : new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
