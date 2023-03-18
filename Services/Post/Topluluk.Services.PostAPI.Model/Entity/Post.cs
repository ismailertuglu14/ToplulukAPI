using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class Post : AbstractEntity
	{
		// Post mutlaka bir kullanıcı tarafından paylaşılacaktır.
		// Paylaşılan post Topluluk altında paylaşılmışsa CommunityId değerine id atanacaktır.
		// todo Burada dikkat edilmesi gereken bir nokta var.
		// Eğer topluluk altında paylaşılmışsa ve topluluk private, restricted, veya sonradan kapatılmış ise
		// bu post duruma göre silinecek veyahut topluluğu takip etmeyen kullanıcılar
		// tarafından görülemeyecektir.
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
		// For statistics 
		public ICollection<string> SavedBy { get; set; }
		//public Dictionary<string, int> Viewing { get; set; }

		public Post()
		{
			Interactions = new HashSet<InteractionType>();
			SharedBy = new HashSet<string>();
			SavedBy = new HashSet<string>();
			Files = new HashSet<string>();
			//Viewing = new Dictionary<string, int>();
		}
	}
}

