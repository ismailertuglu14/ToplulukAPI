using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class UserDeleteDto
	{
		public string userId { get; set; }
		public string Password { get; set; } = string.Empty;
	}
}

