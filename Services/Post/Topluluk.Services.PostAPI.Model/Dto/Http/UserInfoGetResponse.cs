using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto.Http
{
	public class UserInfoGetResponse
	{
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImage { get; set; }
        public GenderEnum Gender { get; set; }

        public bool IsUserFollowing { get; set; }
    }
}

