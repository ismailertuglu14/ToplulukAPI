using System;
namespace Topluluk.Services.AuthenticationAPI.Model.Dto.Http
{
	public class UserDeleteDto
	{
		public string UserId { get; set; }
		public string Password { get; set; }
		public UserDeleteDto()
		{
		}
	}
}

