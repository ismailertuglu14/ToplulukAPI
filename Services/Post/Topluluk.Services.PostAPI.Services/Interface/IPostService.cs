using System;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Services.Interface
{
	public interface IPostService
	{
		Task<Response<string>> Create(CreatePostDto postDto);

		Task<Response<string>> Update();

        Task<Response<string>> Delete(string postId);

		Task<Response<string>> Interaction(string postId, InteractionType interactionType);

		Task<Response<string>> Comment(string userId, string postId, string comment);
		// Her kullanıcı kendi yorumunu silebilir.
		// Adminler herkesin yorumunu silebilir.
		Task<Response<string>> DeleteComment(string userId, string commentId);
		Task<Response<string>> UpdateComment(string userId, string commentId, string newComment);

        

    }
}

