using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.User.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }
        [HttpGet]
        public async Task<Response<string>> GetById(string id)
        {
            return await _userService.GetUserById(id);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Index()
        {
            //var httpClient = new HttpClient();
            //var content = new { userName = "string", password = "string" };
            //var content2 = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            //var response = await httpClient.PostAsync("https://localhost:7231/authentication/signin",content2);
            return Ok($"Calisiyor....");
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> InsertUser( [FromBody] UserInsertDto userInfo)
        {
            return await _userService.InsertUser(userInfo);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> ChangeProfileImage(IFormFileCollection files)
        { 
            return await _userService.ChangeProfileImage(UserName, files);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> FollowUser( [FromBody] UserFollowDto userFollowInfo)
        {
            return await _userService.FollowUser(userFollowInfo);
        }


        // For Http Calls coming from other services

        // When the User joins the community
        [NonAction]
        [CapSubscribe("community.user.communityjoin")]
        public async Task<Response<string>> UpdateCommunities( [FromBody] UserUpdateCommunitiesDto userInfo)
        {
            Console.WriteLine(userInfo);
            return await _userService.UpdateCommunities(userInfo.UserId, userInfo.CommunityId);
        }


        // If the community the user is requesting to join is private
        [NonAction]
        [CapSubscribe("community.user.communityjoinrequest")]
        public async Task<Response<string>> UpdateCommunitiesRequest([FromBody] UserUpdateCommunitiesDto userInfo)
        {
            Console.WriteLine(userInfo);
            return await _userService.UpdateCommunities(userInfo.UserId, userInfo.CommunityId);
        }

        // Update User communities property after user create a new community
        [NonAction]
        [CapSubscribe(QueueConstants.COMMUNITY_CREATE_USER_UPDATE)]
        public async Task<Response<string>> UpdateUserCommunitiesAfterCreate([FromBody] UserUpdateCommunitiesDto userInfo)
        {
            return await _userService.UpdateCommunities(userInfo.UserId, userInfo.CommunityId);
        }
    }
}

