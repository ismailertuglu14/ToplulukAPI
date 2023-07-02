using System;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CommentGetDto
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }
		public GenderEnum Gender { get; set; }

		public string Message { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool IsEdited { get; set; }

		public int ReplyCount { get; set; }
		public CommentInteracted IsInteracted { get; set; }
		public CommentLikes InteractionCounts { get; set; }
		public CommentGetDto()
		{
			
		}
	}

	public class CommentInteracted
	{
		public bool Like { get; set; }
		public bool Dislike { get; set; }
	}

	public class CommentLikes
	{
		public int LikeCount { get; set; }
		public int DislikeCount { get; set; }
	}
}

