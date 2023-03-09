using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CreatePostDto
	{
		public string? UserId { get; set; }
		public string? CommunityId { get; set; }
		public string Description { get; set; }
		public IFormFileCollection? Files { get; set; }
	}
}

