using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Dto.Http;
using Topluluk.Services.AuthenticationAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.AuthenticationAPI.Controllers
{ 
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("[action]")]
        public async Task<Response<TokenDto>> SignIn(SignInUserDto userDto)
        {
            string? ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            string? deviceId = Request.Headers["User-Agent"];
            return await _authenticationService.SignIn(userDto,ipAddress,deviceId);
        }

        [HttpPost("[action]")]
        public async Task<Response<TokenDto>> SignUp(CreateUserDto userDto)
        {
            return await _authenticationService.SignUp(userDto);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> SignOut(SignOutUserDto userDto)
        {
            return await _authenticationService.SignOut(this.UserId, userDto);
        }

        // @@@@@@@@@@@ Http Requests @@@@@@@@@@@@@@@

        [HttpPost("delete")]
        public async Task<Response<string>> Delete(UserDeleteDto dto)
        {

            return await _authenticationService.DeleteUser(this.UserId, dto);
        }

        [HttpPost("change-password")]
        public async Task<Response<string>> ChangePassword(PasswordChangeDto passwordDto)
        {
            return await _authenticationService.ChangePassword(this.UserId, passwordDto);
        }

        [HttpPost("update-profile")]
        public async Task<Response<NoContent>> UpdateProfile(UserUpdateDto userDto)
        {
            return await _authenticationService.UpdateProfile(this.UserId, userDto);
        }

    }
}

