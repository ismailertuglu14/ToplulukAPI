using System;
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

		public int InteractionCount { get; set; }
		public bool IsLiked { get; set; } = false;
		
		public bool IsEdited { get; set; }

		public int ReplyCount { get; set; }
		public CommentGetDto()
		{
			
		}
	}
}

