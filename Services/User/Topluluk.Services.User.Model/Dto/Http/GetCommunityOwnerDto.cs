using System;
namespace Topluluk.Services.User.Model.Dto.Http
{
	public class GetCommunityOwnerDto
	{
		public string OwnerId { get; set; }
		public string Name { get; set; }
		public string ProfileImage { get; set; }
		public GetCommunityOwnerDto()
		{
		}
	}
}

