using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class GetUserByIdDto
	{
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public string? BannerImage { get; set; }

        public bool IsPrivate { get; set; } = false;

        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }

        public ICollection<string>? Communities { get; set; }

        // Kullanıcı profilinde eğer senin admin olduğun topluluklara istek atmışsa
        // Bir row area da kabul et / reddet gibi aksiyon için dönüyoruz.
        public ICollection<string>? CommunityRequests { get; set; }
        public ICollection<string>? Posts { get; set; }

        public GetUserByIdDto()
        {
            Communities = new HashSet<string>();
            CommunityRequests = new HashSet<string>();
            Posts = new HashSet<string>();
        }
    }
}

