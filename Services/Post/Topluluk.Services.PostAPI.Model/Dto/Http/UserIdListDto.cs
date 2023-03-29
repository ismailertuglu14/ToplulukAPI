using System;
namespace Topluluk.Services.PostAPI.Model.Dto.Http
{
	public class UserIdListDto
	{
		public List<string> Ids { get; set; }
		public UserIdListDto()
		{
			Ids = new List<string>();
		}
	}
}

