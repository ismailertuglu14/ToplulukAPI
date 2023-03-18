using System;
namespace Topluluk.Services.EventAPI.Model.Dto.Http
{
	public class GetCommunityByIdDto
	{
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminImage { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string? Location { get; set; }

        public string? CoverImage { get; set; }
        public string? BannerImage { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool IsPublic { get; set; } = true;
        public bool IsRestricted { get; set; } = false;

        public int ParticipiantsCount { get; set; }

        public bool? IsOwner { get; set; }

    }
}

