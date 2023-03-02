using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Model.Dto
{
	public class CommunityCreateDto
	{
        public string? CreatedById { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public IFormFile? CoverImage { get; set; }
        //public IFormFile? BannerImage { get; set; }
        public bool IsVisible { get; set; } = true;
        // Kullanıcılar topluluğa katılabilmek için izin istemek zorunda mı ?
        public bool IsPublic { get; set; } = true;
        public int? ParticipiantLimit { get; set; }
        //public ICollection<Question>? Questions { get; set; }

    }
}

