using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class UserUpdateProfileDto
	{

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string? Bio { get; set; }
		public string Title { get; set; }
		public GenderEnum Gender { get; set; }
		public DateTime BirthdayDate { get; set; }
	}
}

