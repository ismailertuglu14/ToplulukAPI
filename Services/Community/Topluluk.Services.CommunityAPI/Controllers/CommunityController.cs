using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.CommunityAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunityController : BaseController
    {
        
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet("communities/all")]
        public async Task<Response<List<Community>>> GetCommunities()
        {
            return await _communityService.GetCommunities();
        }

        /// <summary>
        /// https://topluluk.com/community/communities?skip=0&take=5
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet("communities")]
        public async Task<Response<List<object>>> GetCommunitiySuggestions(int skip, int take)
        {
            return await _communityService.GetCommunitySuggestions(skip,take,Request);
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> Join(CommunityJoinDto communityInfo)
        {
            communityInfo.UserId = GetUserId();
            return await _communityService.Join(communityInfo);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> Create([FromForm] CommunityCreateDto communityInfo)
        {
            communityInfo.CreatedById = GetUserId();
            return await _communityService.Create(communityInfo);
        }

        //63ff7d52ec2541d3daa3a86b
        [HttpPost("[action]")]
        public async Task<Response<string>> Delete(string id)
        {
            return await _communityService.Delete(id);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo)
        {
            dtoInfo.AdminId = GetUserId();
            return await _communityService.AssignUserAsAdmin(dtoInfo);
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo)
        {
            dtoInfo.AssignedById = GetUserId();
            return await _communityService.AssignUserAsModerator(dtoInfo);
        }

        // Http call methods

        [NonAction]
        [CapSubscribe(QueueConstants.COMMUNITY_IMAGE_UPLOADED)]
        public async Task<Response<string>> UpdateCoverImage(CommunityImageUploadedDto dto)
        {
            return await _communityService.UpdateCoverImage(dto);
        }
    }
    
}

