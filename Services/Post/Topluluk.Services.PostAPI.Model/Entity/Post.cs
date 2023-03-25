using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class Post : AbstractEntity
	{
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? ProfileImage { get; set; }
		public string? CommunityId { get; set; }
		public string? SharedById { get; set; }
		public ICollection<string> Files { get; set; }
		public ICollection<string> SharedBy { get; set; }
		public string Description { get; set; }
		public ICollection<InteractionType> Interactions { get; set; }
		public bool IsShownOnProfile { get; set; } = true;

        public string? CommunityLink { get; set; }

        public string? EventLink { get; set; }


		public Post()
		{
			Interactions = new HashSet<InteractionType>();
            SharedBy = new HashSet<string>();
			Files = new HashSet<string>();
			//Viewing = new Dictionary<string, int>();
		}
	}
}

