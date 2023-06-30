using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class PostComment : AbstractEntity
	{
		public string? UserId { get; set; }
		public string PostId { get; set; }
		public string ParentCommentId { get; set; }
		public string Message { get; set; }
		public List<PreviousMessage> PreviousMessages { get; set; }
	}

	public class PreviousMessage
	{
		public string Message { get; set; }
		public DateTime EditedDate { get; set; }
	}
}

