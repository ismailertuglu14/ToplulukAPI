using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class GetUserByIdDto
	{
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public string? BannerImage { get; set; }
        public GenderEnum Gender { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsBlocked { get; set; }
        public bool IsFollowing { get; set; }
        
        ///  Is target user sent to follow request to source user
        public bool isFollowRequestReceived { get; set; }
        
        /// Is source user sent to follow request to target user 
        public bool IsFollowRequestSent { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }

        public ICollection<string>? Communities { get; set; }
        public int CommunityCount { get; set; }

        public ICollection<string>? Posts { get; set; }

        public GetUserByIdDto()
        {
            Communities = new HashSet<string>();
            Posts = new HashSet<string>();
        }
    }
}

