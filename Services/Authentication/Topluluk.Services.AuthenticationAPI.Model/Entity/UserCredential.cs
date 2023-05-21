using System;
using MongoDB.Bson;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.AuthenticationAPI.Model.Entity
{
	public class UserCredential : AbstractEntity
	{
		public List<string> Role { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }
		public bool? EmailConfirmed { get; set; } = false;

		public string? PhoneNumber { get; set; }
		public bool? PhoneNumberConfirmed { get; set; } = false;

		public string HashedPassword { get; set;}

		public bool? TwoFactorEnabled { get; set; } = false;

		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenEndDate { get; set; }

		// Specifies the user's registration method.
		public LoginProvider Provider { get; set; } = 0;

		public int? AccessFailedCount { get; set; } = 0;
		public bool? Locked { get; set; } = false;
		public DateTime LockoutEnd { get; set; }
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordTokenEndDate { get; set; }

        public UserCredential()
        {
	        Role = new List<string>(){UserRoles.USER};
			Id = ObjectId.GenerateNewId().ToString();
		}
	}
}

