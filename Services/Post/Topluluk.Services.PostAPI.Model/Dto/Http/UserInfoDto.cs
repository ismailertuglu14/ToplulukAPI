
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto.Http
{
	public class UserInfoDto
	{
		public string Id { get; set; }
		public string? ProfileImage { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public GenderEnum Gender { get; set; }
		
	}
}

