using System;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto
{
	public class GetPostByIdDto
	{
		public string Id { get; set; }

		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string? ProfileImage { get; set; }
		public GenderEnum Gender { get; set; }

		public string? CommunityTitle { get; set; }

		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }

		// Kullanıcı postu paylaşan kişiyi takip ediyor mu?
		public bool IsUserFollowing { get; set; }

		public int InteractionCount { get; set; }

		// Sadece tanıdığı takip ettiği kişileri öncelikli olarak gösterebilmek adına kullanıyoruz.
		// Eğer tanıdığı kişi yok ise listenin ilk elemanını gösterebiliriz.
		public ICollection<InteractionType>? InteractedByIds { get; set; }

		public ICollection<string>? Files { get; set; }

		public int CommentCount { get; set; }
		
		
		// todo Change PostComment to CommendtDto.
		public ICollection<CommentGetDto>? Comments { get; set; }

		public GetPostByIdDto()
		{
			InteractedByIds = new HashSet<InteractionType>();
            Files = new HashSet<string>();
			Comments = new HashSet<CommentGetDto>();
		}
	}
}

