using System;
namespace Topluluk.Services.AuthenticationAPI.Model.Dto
{
	public class CreateUserDto
	{
		// this 2 properties for UserCollection
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string UserName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}

