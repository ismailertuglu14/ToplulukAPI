using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class Comment : AbstractEntity
	{
		public string? UserId { get; set; }
		public string PostId { get; set; }
		public string Message { get; set; }
		public ICollection<InteractionType> Interactions { get; set; }
		public ICollection<Comment> Replies { get; set; }

		public Comment()
		{
			Replies = new HashSet<Comment>();
			Interactions = new HashSet<InteractionType>();
		}

	}
}

