using System;
namespace Topluluk.Services.CommunityAPI.Model.Dto
{
	public class CommunityGetPreviewDto
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public int ParticipiantsCount { get; set; }
		public string? CoverImage { get; set; }
		public bool IsPrivate { get; set; }

		public CommunityGetPreviewDto()
		{
		}
	}
}

