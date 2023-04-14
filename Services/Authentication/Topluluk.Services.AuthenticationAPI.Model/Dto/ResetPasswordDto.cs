using System;
namespace Topluluk.Services.AuthenticationAPI.Model.Dto
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
        public string NewPasswordAgain { get; set; }
    }
}

