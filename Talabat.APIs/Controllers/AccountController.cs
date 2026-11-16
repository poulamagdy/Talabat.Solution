using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extantions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenServices _tokenServices;
		private readonly IMapper _mapper;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
			ITokenServices tokenServices, IMapper mapper)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenServices = tokenServices;
			_mapper = mapper;
		}
        [HttpPost("Register")]
		public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
		{
			if (CheackEmailExist(model.Email).Result.Value)
			{
				return BadRequest(new ApiResponse(400, "Email Is Already Exist"));
			}
			var user = new AppUser()
			{
				UserName = model.Email.Split('@')[0],
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				DisplayName = model.DisplayName,
			};

			var result =  await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

			var registerDto = new UserDTO()
			{
				Email = user.Email,
				DisplayName= user.DisplayName,
				Token = await _tokenServices.CreateTokenAsync(user, _userManager)
			};

			return Ok(registerDto);
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is null) return Unauthorized(new ApiResponse(401));

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

			var loginDto = new UserDTO()
			{
				Email = user.Email,
				DisplayName = user.DisplayName,
				Token = await _tokenServices.CreateTokenAsync(user, _userManager)
			};

			return Ok(loginDto);
		}

		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDTO>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			var user = await _userManager.FindByEmailAsync(email);

			var userDto = new UserDTO
			{
				Email = user.Email,
				DisplayName = user.DisplayName,
				Token = await _tokenServices.CreateTokenAsync(user, _userManager),
			};

			return Ok(userDto);
		}

		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDTO>> GetAddressOfUser()
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

			var addressDto = _mapper.Map<AddressDTO>(user.Address);

			return Ok(addressDto);
		}

		[Authorize]
		[HttpPost("Address")]
		public async Task<ActionResult<AddressDTO>> UpdateAdress(AddressDTO model)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

			var mapperAddress = _mapper.Map<Address>(model);

			mapperAddress.Id = user.Address.Id;

			user.Address = mapperAddress;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

			return Ok(model);
		}

		[HttpGet("EmailExist")]
		public async Task<ActionResult<bool>> CheackEmailExist(string email)
			=> await _userManager.FindByEmailAsync(email) is not null;
	}
}
