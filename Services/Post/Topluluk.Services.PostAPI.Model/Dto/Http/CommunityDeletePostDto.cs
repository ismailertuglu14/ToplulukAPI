using System;
namespace Topluluk.Services.PostAPI.Model.Dto.Http
{
	public class CommunityDeletePostDto
	{
		public string PostId { get; set; }
		public string UserId { get; set; }
		public string CommunityId { get; set; }
	}
}

