using System;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class GetPostDto
	{
		public string UserId { get; set; }
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string? ProfileImage { get; set; }

		public string Description { get; set; }
		public DateTime SharedAt { get; set; }
		// Oran orantı kur.
		public int InteractionCount { get; set; }
		public int CommentsCount { get; set; }
	}
}

