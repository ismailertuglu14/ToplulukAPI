﻿using System;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Services.Interface
{
	public interface IPostService
	{
        Task<Response<List<GetPostDto>>> GetPosts(string userId, int take = 10, int skip = 0 );
        Task<Response<GetPostByIdDto>> GetPostById(string postId, string sourceUserId, bool isDeleted = false);
        Task<Response<string>> GetCommunityPosts(string communityId, int skip = 0, int take = 10);
        Task<Response<List<GetPostForFeedDto>>> GetUserPosts(string userId, string id, int take = 10, int skip = 0);


        Task<Response<List<GetPostForFeedDto>>> GetPostForFeedScreen(string userId, string token, int skip = 0, int take = 10);


        Task<Response<string>> Create(string userId, CreatePostDto postDto);

		Task<Response<string>> Update();

        Task<Response<string>> Delete(PostDeleteDto postDto);

		Task<Response<string>> Interaction(string userId, string postId, PostInteractionCreateDto interactionCreate);
		Task<Response<string>> RemoveInteraction(string userId, string postId);

        Task<Response<List<GetPostForFeedDto>>> GetSavedPosts(string userId, int take = 10, int skip = 0);
        Task<Response<string>> SavePost(string userId, string postId);

		Task<Response<string>> Comment(CommentCreateDto commentDto);
        Task<Response<List<CommentGetDto>>> GetComments(string userId, string postId, int take = 10, int skip = 0);
        // Her kullanıcı kendi yorumunu silebilir.
        // Adminler herkesin yorumunu silebilir.
        Task<Response<string>> DeleteComment(string userId, string commentId);
		Task<Response<string>> UpdateComment(string userId, string commentId, string newComment);



        // Http Calls

        Task<Response<bool>> DeletePosts(string userId);
     }
}

