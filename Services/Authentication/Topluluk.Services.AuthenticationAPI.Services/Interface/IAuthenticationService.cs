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
        Task<Response<string>> SignOut(string userId, SignOutUserDto userDto);
        Task<Response<string>> ResetPasswordRequest(string email);
        Task<Response<bool>> VerifyResetToken(string userId, string resetToken);
        Task<Response<NoContent>> ResetPassword(string userId, string resetToken, ResetPasswordDto passwordDto);
        // Http Request
        Task<Response<string>> DeleteUser(string id, UserDeleteDto userDto);
        Task<Response<string>> ChangePassword(string userId, PasswordChangeDto passwordDto);
		Task<Response<NoContent>> UpdateProfile(string userId, UserUpdateDto userDto);
	}
}

