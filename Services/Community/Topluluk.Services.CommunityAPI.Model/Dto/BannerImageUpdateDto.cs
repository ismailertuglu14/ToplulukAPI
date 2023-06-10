using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.CommunityAPI.Model.Dto
{
    public class BannerImageUpdateDto
    {
        public IFormFile File { get; set; }
    }
}

