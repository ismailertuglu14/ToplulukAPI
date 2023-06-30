using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.CommunityAPI.Controllers
{
    
    public class CommunityController : BaseController
    {

        private readonly ICommunityService _communityService;
        
        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }
    
        [HttpGet("communities")]
        public async Task<Response<List<CommunityGetPreviewDto>>> GetCommunitiySuggestions(int skip, int take)
        {
            return await _communityService.GetCommunitySuggestions(this.UserId,Request,skip, take);
        }
        

        [HttpGet("{id}")]
        public async Task<Response<CommunityGetByIdDto>> GetCommunityById(string id)
        {
            return await _communityService.GetCommunityById(UserId, this.Token, id);
        }

        [HttpPost("create")]
        public async Task<Response<string>> Create([FromForm] CommunityCreateDto communityInfo)
        {

            return await _communityService.Create(this.UserId, this.Token, communityInfo);
        }


        [HttpPost("delete")]
        public async Task<Response<string>> Delete(string id)
        {
            return await _communityService.Delete(this.UserId,id);
        }


        
        [HttpPost("delete-permanently/{id}")]
        public async Task<Response<string>> DeletePermanently(string id)
        {
            return await _communityService.DeletePermanently(UserName, id);
        }



        [HttpPost("assign-user-as-admin")]
        public async Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo)
        {
            return await _communityService.AssignUserAsAdmin(this.UserId, dtoInfo);
        }
        
        [HttpPost("assign-user-as-moderator")]
        public async Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo)
        {
            dtoInfo.AssignedById = UserId;
            return await _communityService.AssignUserAsModerator(dtoInfo);
        }

        [HttpPost("kick-user/{communityId}/{userId}")]
        public async Task<Response<NoContent>> KickUser(string communityId, string userId)
        {
            return await _communityService.KickUser(this.Token, communityId, userId);
        }
        
     

        /// <summary>
        /// Returns information of the communities the user has joined.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet("user-communities")]
        public async Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities(string id, int skip, int take)
        {
            return await _communityService.GetUserCommunities(id,skip,take);
        }
        
        /// <summary>
        /// Returns count of the communities the user has joined.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("user-communities-count")]
        public async Task<Response<int>> GetUserParticipiantCommunityCount(string id)
        {
            return await _communityService.GetUserParticipiantCommunitiesCount(id);
        }
    


        [HttpGet("getCommunityTitle")]
        public async Task<Response<string>> GetCommunityTitle(string id)
        {
            return await _communityService.GetCommunityTitle(id);
        }

        [HttpGet("check-exist")]
        public async Task<Response<bool>> CheckCommunityExist(string id)
        {
            return await _communityService.CheckCommunityExist(id);
        }

        
        [HttpGet("check-is-user-community-owner")]
        public async Task<Response<bool>> CheckIsUserCommunityOwner()
        {
            return await _communityService.CheckIsUserAdminOwner(this.UserId);
        }
        
        [HttpGet("community-info-post-link")]
        public async Task<Response<CommunityInfoPostLinkDto>> GetCommunityInfoForPostLink(string id)
        {
            return await _communityService.GetCommunityInfoForPostLink(id);
        }
    }
    
}

