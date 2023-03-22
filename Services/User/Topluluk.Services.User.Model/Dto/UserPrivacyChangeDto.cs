using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class UserPrivacyChangeDto
	{
		public string UserId { get; set; } = string.Empty;
		public bool IsPrivate { get; set; } = false;
		public UserPrivacyChangeDto()
		{
		}
	}
}

