using System;
namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class PostDeleteDto
	{
		public string PostId { get; set; }
		public string? UserId { get; set; }
	}
}

