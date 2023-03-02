using System;
namespace Topluluk.Services.CommunityAPI.Model.Dto
{
	public class AssignUserAsModeratorDto
	{
        public string UserId { get; set; }
        public string? AssignedById { get; set; }
        public string CommunityId { get; set; }
    }
}

