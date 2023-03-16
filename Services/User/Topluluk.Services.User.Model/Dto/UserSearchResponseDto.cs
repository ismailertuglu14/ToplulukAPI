using System;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto
{
	public class UserSearchResponseDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string ProfileImage { get; set; }
		public bool IsPrivate { get; set; }
		public GenderEnum Gender { get; set; }
		public SearchResponseType Type { get; set; } = SearchResponseType.USER;

		public UserSearchResponseDto()
		{
			Gender = GenderEnum.Unspecified;
		}

	}
}

