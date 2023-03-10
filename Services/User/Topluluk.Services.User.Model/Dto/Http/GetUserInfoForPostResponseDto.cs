using System;
namespace Topluluk.Services.User.Model.Dto.Http
{
	public class GetUserInfoForPostResponseDto
	{
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string? ProfileImage { get; set; }

		public bool IsUserFollowing { get; set; }
    }
}

