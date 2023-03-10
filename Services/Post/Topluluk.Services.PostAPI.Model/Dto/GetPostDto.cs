using System;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class GetPostDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }

		public string Description { get; set; }
		public DateTime SharedAt { get; set; }
		public List<InteractionType> Interactions { get; set; }
		public int SharedCount { get; set; }
		public int CommentsCount { get; set; }
		public List<PostComment> Comments { get; set; }

		public GetPostDto()
		{
			Comments = new List<PostComment>();
			Interactions = new List<InteractionType>();
		}
	}
}

