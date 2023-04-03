﻿using System;
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
            string ipAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string deviceId = Request.Headers["User-Agent"];
            return await _authenticationService.SignIn(userDto,ipAdress,deviceId);
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

        // Delete user from http request
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

    }
}

