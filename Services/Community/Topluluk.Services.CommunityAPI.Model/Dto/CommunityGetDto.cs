using System;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Model.Dto
{
	public class CommunityGetDto
	{
        public ICollection<Moderator> ModeratorIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string? CoverImage { get; set; }
        public string? BannerImage { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool IsPublic { get; set; } = true;
        public bool IsRestricted { get; set; } = false;

        public int ParticipiantsCount { get; set; }

        public bool? IsOwner { get; set; }
    }
    public class CommunityGetAdminDto
    {

    }
}

