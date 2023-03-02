using System;
using System.Threading.Tasks;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Services.Interface
{
	public interface IAuthenticationService
	{
		Task<Response<TokenDto>> SignIn(SignInUserDto userDto);
		Task<Response<string>> SignUp(CreateUserDto userDto);
	}
}

