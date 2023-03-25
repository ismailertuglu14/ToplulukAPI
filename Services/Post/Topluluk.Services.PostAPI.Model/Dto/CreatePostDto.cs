using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class CreatePostDto
	{
		public string? UserId { get; set; }
		public string? CommunityId { get; set; }
        public string? CommunityLink { get; set; }
        public string? EventLink { get; set; }

		[MaxLength(1024)]
		public string Description { get; set; }
		public bool IsShownOnProfile { get; set; } = true;
		public IFormFileCollection? Files { get; set; }
		
	}
}

