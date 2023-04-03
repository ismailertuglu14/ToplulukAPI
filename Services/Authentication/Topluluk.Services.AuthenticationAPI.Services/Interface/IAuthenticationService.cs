﻿using System;
using System.Threading.Tasks;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Services.Interface
{
	public interface IAuthenticationService
	{
		Task<Response<TokenDto>> SignIn(SignInUserDto userDto, string ipAdress, string deviceId);
		Task<Response<TokenDto>> SignUp(CreateUserDto userDto);
        Task<Response<string>> SignOut(string userId, SignOutUserDto userDto);
        Task<Response<string>> ResetPassowrd();
        Task<Response<string>> ChangePassword(string userId, PasswordChangeDto passwordDto);
        Task<Response<string>> DeleteUser(string id, UserDeleteDto userDto);
	}
}

