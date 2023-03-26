using System;
using System.Threading.Tasks;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Services.Interface
{
	public interface IAuthenticationService
	{
		Task<Response<TokenDto>> SignIn(SignInUserDto userDto);
		Task<Response<TokenDto>> SignUp(CreateUserDto userDto);
        Task<Response<string>> SignOut(string userId, SignOutUserDto userDto);
		Task<Response<string>> DeleteUser(string id, UserDeleteDto userDto);
	}
}

