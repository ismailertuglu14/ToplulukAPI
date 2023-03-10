using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class GetUserByTokenDto
	{

		public string Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }

		public string ProfileImage { get; set; }
		public string BannerImage { get; set; }

		public Int16 FollowingsCount { get; set; }
		public Int16 FollowersCount { get; set; }

		public ICollection<string> FollowingRequests { get; set; }

		public GetUserByTokenDto()
		{
			FollowingRequests = new HashSet<string>();
		}
	}
}

