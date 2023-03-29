using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class FollowingRequestDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
        public string? ProfileImage { get; set; }
		public GenderEnum Gender { get; set; }

        public FollowingRequestDto()
		{
			
			Gender = GenderEnum.Unspecified;
		}
	}
}

