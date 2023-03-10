using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class PostComment : AbstractEntity
	{
		public string? UserId { get; set; }
		public string PostId { get; set; }
		public string Message { get; set; }
		public ICollection<InteractionType> Interactions { get; set; }
		public ICollection<PostComment> Replies { get; set; }

		public PostComment()
		{
			Replies = new HashSet<PostComment>();
			Interactions = new HashSet<InteractionType>();
		}

	}
}

