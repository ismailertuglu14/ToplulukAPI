﻿using System;
using System.Collections.Generic;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Entity
{
	public class User : AbstractEntity
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
        public string Email { get; set; }
        
        public string? Title { get; set; }
        public string? Bio { get; set; }
		public string? ProfileImage { get; set; }
		public string? BannerImage { get; set; }

		public GenderEnum? Gender { get; set; }

		public DateTime? BirthdayDate { get; set; }

		public bool IsPrivate { get; set; } = false;

		public ICollection<string> BlockedUsers { get; set; }
		public ICollection<string> IncomingFollowRequests { get; set; }
		public ICollection<string> OutgoingFollowRequests { get; set; }


		public User()
		{
			IncomingFollowRequests = new HashSet<string>();
			OutgoingFollowRequests = new HashSet<string>();
			BlockedUsers = new HashSet<string>();
			Gender = GenderEnum.Unspecified;
        }

	}
}

