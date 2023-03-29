using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class FollowersUserDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string? ProfileImage { get; set; }
		public bool IsFollowing { get; set; }

		public FollowersUserDto()
		{
		}
	}
}

