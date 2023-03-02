using System;
using MongoDB.Bson;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Model.Entity
{
	public class UserCredential : AbstractEntity
	{
		public string UserName { get; set; }

		public string Email { get; set; }
		public bool? EmailConfirmed { get; set; } = false;

		public string? PhoneNumber { get; set; }
		public bool? PhoneNumberConfirmed { get; set; } = false;

		public string HashedPassword { get; set;}

		public bool? TwoFactorEnabled { get; set; } = false;

		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenEndDate { get; set; }

		// Lock at 3rd attempt.
		public int? AccessFailedCount { get; set; } = 0;
		public bool? Locked { get; set; } = false;
		public DateTime LockoutEnd { get; set; }

		public UserCredential()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}
	}
}

