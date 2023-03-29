using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class UserFollowerRequestDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileImage { get; set; }
		public DateTime CreatedAt { get; set; }

		public UserFollowerRequestDto()
		{
		}
	}
}

