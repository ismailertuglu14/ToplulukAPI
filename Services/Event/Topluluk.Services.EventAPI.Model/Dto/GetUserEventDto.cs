using System;
namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class GetUserEventDto
	{
		public string UserId { get; set; }
		public string EventId { get; set; }
		public string CoverImage { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Title { get; set; }
		public string Location { get; set; }

		public GetUserEventDto()
		{
		}
	}
}

