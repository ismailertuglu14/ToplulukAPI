using System;
namespace Topluluk.Shared.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}

