using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class GetUserAfterLoginDto
    {
		public string Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public GenderEnum Gender { get; set; }

		public string? ProfileImage { get; set; }
		public string? BannerImage { get; set; }

		public int FollowingsCount { get; set; }
		public int FollowersCount { get; set; }
	
		public ICollection<FollowingRequestDto>? FollowingRequests { get; set; }
	
		//public ICollection<string> Notifications { get; set; }

		public GetUserAfterLoginDto()
		{
			FollowingRequests = new HashSet<FollowingRequestDto>();
		}
	}
}

