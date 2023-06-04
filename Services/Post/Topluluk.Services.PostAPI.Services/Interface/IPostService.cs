using System;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Services.Interface
{
	public interface IPostService
	{
		//Test
		
		
        Task<Response<GetPostByIdDto>> GetPostById(string postId, string sourceUserId, bool isDeleted = false);
        Task<Response<List<GetPostForFeedDto>>> GetCommunityPosts(string userId, string communityId, int skip = 0, int take = 10);
        Task<Response<List<GetPostForFeedDto>>> GetUserPosts(string userId, string id, int take = 10, int skip = 0);

        Task<Response<List<GetPostForFeedDto>>> GetPostForFeedScreen(string userId, string token, int skip = 0, int take = 10);


        Task<Response<string>> Create(string userId, CreatePostDto postDto);

		Task<Response<string>> Update();

        Task<Response<string>> Delete(PostDeleteDto postDto);
        
        
		Task<Response<string>> Interaction(string userId, string postId, PostInteractionCreateDto interactionCreate);
		Task<Response<List<UserInfoDto>>> GetInteractions(string userId, string postId, int type = 0, int take = 10, int skip = 0);
		Task<Response<string>> RemoveInteraction(string userId, string postId);

        
		Task<Response<List<GetPostForFeedDto>>> GetSavedPosts(string userId, int take = 10, int skip = 0);
        Task<Response<string>> SavePost(string userId, string postId);

		

        // Http Calls
        Task<Response<bool>> DeletePosts(string userId);
     }
}

