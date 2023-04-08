using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.AuthenticationAPI.Model.Dto
{
	public class SignInUserDto
	{
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public LoginProvider Provider { get; set; }
		
	}
}

