using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class GetUserAfterLoginDto
    {
		public string Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public GenderEnum Gender { get; set; }
        public string Bio { get; set; }

        public string? ProfileImage { get; set; }
		public string? BannerImage { get; set; }

		public DateTime BirthdayDate { get; set; }
		
		public int FollowingsCount { get; set; }
		public int FollowersCount { get; set; }
		public bool IsPrivate { get; set; }
		

		public GetUserAfterLoginDto()
		{
			
		}
	}
}

