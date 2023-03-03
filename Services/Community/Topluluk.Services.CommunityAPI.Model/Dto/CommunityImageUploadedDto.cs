using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.CommunityAPI.Model.Dto
{
    public class CommunityImageUploadedDto
    {
        public string CommunityId { get; set; }
        public string? CoverImage { get; set; }
    }
}

