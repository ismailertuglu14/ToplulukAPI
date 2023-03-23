using System;
namespace Topluluk.Services.EventAPI.Model.Dto
{
	public class GetEventByIdDto
	{
		public string Title { get; set; }
		public string Descriptions { get; set; }
		public List<string> Images { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Location { get; set; }
		public int AttendeesCount { get; set; }
		public bool IsAttendeed { get; set; } = false;
		public int CommentCount { get; set; }

		public GetEventByIdDto()
		{
			Images = new List<string>();
		}
	}
}

