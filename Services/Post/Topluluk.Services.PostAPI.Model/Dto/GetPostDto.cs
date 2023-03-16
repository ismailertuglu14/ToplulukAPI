using System;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class GetPostDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }

		public string Description { get; set; }
		public DateTime SharedAt { get; set; }
		// Oran orantı kur.
		public List<InteractionType> Interactions { get; set; }

		public int CommentsCount { get; set; }

		public GetPostDto()
		{
			Interactions = new List<InteractionType>();
		}
	}
}

