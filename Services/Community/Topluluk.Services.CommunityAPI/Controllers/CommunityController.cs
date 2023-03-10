using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
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
            return await _communityService.GetCommunitySuggestions(skip, take, Request);
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> Join(CommunityJoinDto communityInfo)
        {
            communityInfo.UserId = UserId;
            return await _communityService.Join(communityInfo);
        }

        [HttpGet("{id}")]
        public async Task<Response<string>> GetCommunityById(string id)
        {
            return await _communityService.GetCommunityById(UserId,id);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> Create([FromForm] CommunityCreateDto communityInfo)
        {
            communityInfo.CreatedById = UserId;
            return await _communityService.Create(communityInfo);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> Delete(string id)
        {
            return await _communityService.Delete(id);
        }

        [HttpPost("[action]/{id}")]
        public async Task<Response<string>> DeletePermanently(string id)
        {
            return await _communityService.DeletePermanently(UserName, id);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo)
        {
            dtoInfo.AdminId = UserId;
            return await _communityService.AssignUserAsAdmin(dtoInfo);
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo)
        {
            dtoInfo.AssignedById = UserId;
            return await _communityService.AssignUserAsModerator(dtoInfo);
        }

        // Http call methods

        [NonAction]
        [CapSubscribe(QueueConstants.COMMUNITY_IMAGE_UPLOADED)]
        public async Task<Response<string>> UpdateCoverImage(CommunityImageUploadedDto dto)
        {
            return await _communityService.UpdateCoverImage(dto);
        }
        [HttpGet("Participiants/{id}")]
        public async Task<List<string>> GetParticipiants(string id)
        {
            var response = await _communityService.GetParticipiants(id);
            return response.Data;
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> PostCreated(PostCreatedCommunityDto dtoInfo)
        {
            return await _communityService.PostCreated(dtoInfo);
        }

        [HttpGet("getCommunityTitle")]
        public async Task<Response<string>> GetCommunityTitle(string id)
        {
            return await _communityService.GetCommunityTitle(id);
        }
    }
    
}

