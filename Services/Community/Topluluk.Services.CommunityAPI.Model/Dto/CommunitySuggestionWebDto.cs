using System;
namespace Topluluk.Services.CommunityAPI.Model.Dto
{
	public class CommunitySuggestionWebDto
	{
        public string Id { get; set; }
        public string Title { get; set; }
        public int ParticipiantCount { get; set; }
        public string? CoverImage { get; set; }

    }
}

