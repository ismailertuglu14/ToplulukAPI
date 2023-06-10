using System;
using System.Threading.Tasks;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Services.Interface
{
	public interface IAuthenticationService
	{
		Task<Response<TokenDto>> SignIn(SignInUserDto userDto, string? ipAdress, string? deviceId);
		Task<Response<TokenDto>> SignUp(CreateUserDto userDto);
        Task<Response<NoContent>> SignOut(string userId, SignOutUserDto userDto);
        Task<Response<NoContent>> ResetPasswordRequest(string email);
        Task<Response<NoContent>> ResetPasswordCheckOTP(ResetPasswordCheckOTPDto codeDto);
        Task<Response<NoContent>> ResetPassword(ResetPasswordDto passwordDto);
        // Http Request
        Task<Response<NoContent>> DeleteUser(string id, UserDeleteDto userDto);
        Task<Response<NoContent>> ChangePassword(string userId, PasswordChangeDto passwordDto);
		Task<Response<NoContent>> UpdateProfile(string userId, UserUpdateDto userDto);
	}
}

