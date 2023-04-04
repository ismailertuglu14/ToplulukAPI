﻿using System;
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
using Topluluk.Services.User.Model.Dto.Http;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.API.Controllers
{

    public class UserController : BaseController
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }

        [HttpGet("[action]")]
        public async Task<Response<GetUserByIdDto>> GetUserById(string userId)
        {
            return await _userService.GetUserById(this.UserId, userId);
        }

        [HttpGet("{userName}")]
        public async Task<Response<GetUserByIdDto>> GetUserByUserName([FromRoute] string userName)
        {
            return await _userService.GetUserByUserName(userName);
        }

        [HttpGet("suggestions")]
        public async Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions([FromQuery] int limit)
        {
            return await _userService.GetUserSuggestions(this.UserId, limit);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> InsertUser([FromBody] UserInsertDto userInfo)
        {
            return await _userService.InsertUser(userInfo);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> ChangeProfileImage(IFormFileCollection files)
        {
            return await _userService.ChangeProfileImage(UserName, files);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> ChangeBannerImage([FromForm] UserChangeBannerDto changeBannerDto)
        {
            changeBannerDto.UserId = UserId;
            return await _userService.ChangeBannerImage(changeBannerDto);
        }

        [HttpPost("Follow")]
        public async Task<Response<string>> FollowUser([FromBody] UserFollowDto userFollowInfo)
        {
            userFollowInfo.SourceId = UserId;
            return await _userService.FollowUser(userFollowInfo);
        }

        [HttpPost("UnFollow")]
        public async Task<Response<string>> UnFollowUser([FromBody] UserFollowDto userFollowInfo)
        {
            userFollowInfo.SourceId = UserId;
            return await _userService.UnFollowUser(userFollowInfo);
        }

        [HttpPost("accept-request/{targetId}")]
        public async Task<Response<string>> AcceptFollowRequest(string targetId)
        {
            return await _userService.AcceptFollowRequest(this.UserId, targetId);
        }

        [HttpGet("followings")]
        public async Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, int take, int skip)
        {
            return await _userService.GetFollowingUsers(id, skip, take);
        }

        [HttpGet("follower-requests")]
        public async Task<Response<List<FollowingRequestDto>>> FollowerRequests(int take, int skip)
        {
            return await _userService.GetFollowerRequests(this.UserId, skip, take);
        }

        [HttpPost("Block")]
        public async Task<Response<string>> BlockUser([FromForm] string targetId)
        {
            return await _userService.BlockUser(UserId, targetId);
        }

        [HttpGet("Search")]
        public async Task<Response<List<UserSearchResponseDto>>?> SearchUser([FromQuery] string text, int skip = 0, int take = 5)
        {
            return await _userService.SearchUser(text, this.UserId, skip, take);
        }

        [HttpGet("search-in-followings")]
        public async Task<Response<List<FollowingUserDto>>?> SearchUser(string id, string text, int skip = 0, int take = 10)
        {
            return await _userService.SearchInFollowings(this.UserId, id, text, skip, take);
        }

        [HttpGet("[action]")]
        public async Task<Response<GetUserAfterLoginDto>> GetUserAfterLogin()
        {
            return await _userService.GetUserAfterLogin(this.UserId);
        }

        [HttpPost("delete")]
        public async Task<Response<string>> DeleteUser(UserDeleteDto dto)
        {
            return await _userService.DeleteUserById(this.UserId, this.Token, dto);
        }

        [HttpPost("privacy-change")]
        public async Task<Response<string>> PrivacyChange(UserPrivacyChangeDto dto)
        {
            return await _userService.PrivacyChange(this.UserId, dto);
        }
        [HttpPost("update-profile")]
        public async Task<Response<NoContent>> UpdateProfile(UserUpdateProfileDto dto)
        {
            return await _userService.UpdateProfile(this.UserId, this.Token, dto);
        }
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ For Http Calls coming from other services @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        [HttpPost("community/join")]
        public async Task<Response<NoContent>> JoinCommunity(string communityId)
        {
            return await _userService.JoinCommunity(this.UserId, communityId);
        }

        [HttpPost("community/leave")]
        public async Task<Response<NoContent>> LeaveCommunity( string communityId)
        {
            return await _userService.LeaveCommunity(this.UserId, communityId);
        }

        // User information is received to be displayed on the post cards returned from the post service.
        [HttpGet("GetUserInfoForPost")]
        public async Task<Response<UserInfoGetResponse>> GetUserInfoForPost(string id, string sourceUserId)
        {
            return await _userService.GetUserInfoForPost(id, sourceUserId);
        }

        [HttpGet("user-info-comment")]
        public async Task<Response<UserInfoForCommentDto>> GetUserInfoForComment(string id)
        {
            return await _userService.GetUserInfoForComment(id);
        }

        [HttpGet("communityOwner")]
        public async Task<Response<GetCommunityOwnerDto>> GetCommunityOwner(string id)
        {
            return await _userService.GetCommunityOwner(id);
        }

        [HttpGet("user-followings")]
        public async Task<Response<List<string>>> GetUserFollowings(string id)
        {
            return await _userService.GetUserFollowings(id);
        }

        [HttpPost("get-user-info-list")]
        public async Task<Response<List<GetUserByIdDto>>> GetUserInfoList(UserIdListDto idList, int skip, int take)
        {
            return await _userService.GetUserList(idList, skip, take);
        }
    }
    public class UserBannerChangedDto
    {
        public string UserId { get; set; }
        public string FileName { get; set; }
    }
 
}

