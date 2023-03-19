using System;
namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class FeedEventDto
	{

		public string UserId { get; set; }

		public string EventId { get; set; }
		public string? Location { get; set; }
		public string? CoverImage { get; set; }

		public int AttendeesCount { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public int InteractionCount { get; set; }
		public int CommentCount { get; set; }

		public FeedEventDto()
		{
		}
	}
}

