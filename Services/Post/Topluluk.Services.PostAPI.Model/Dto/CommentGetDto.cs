using System;
namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CommentGetDto
	{
		public string Id { get; set; }
		
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }

		public string Message { get; set; }

		public DateTime CreatedAt { get; set; }

		public int InteractionCount { get; set; }

		public ICollection<CommentGetDto>? Replies { get; set; }

		public CommentGetDto()
		{
			Replies = new HashSet<CommentGetDto>();
		}
	}
}

