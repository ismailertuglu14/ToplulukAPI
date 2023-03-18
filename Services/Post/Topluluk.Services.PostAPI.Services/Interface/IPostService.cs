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
        Task<Response<List<GetPostDto>>> GetUserPosts(string userId, int take = 10, int skip = 0);

        Task<Response<string>> Create(CreatePostDto postDto);

		Task<Response<string>> Update();

        Task<Response<string>> Delete(PostDeleteDto postDto);

		Task<Response<string>> Interaction(string postId, InteractionType interactionType);

		Task<Response<string>> Comment(CommentCreateDto commentDto);
        // Her kullanıcı kendi yorumunu silebilir.
        // Adminler herkesin yorumunu silebilir.
        Task<Response<string>> DeleteComment(string userId, string commentId);
		Task<Response<string>> UpdateComment(string userId, string commentId, string newComment);

    }
}

