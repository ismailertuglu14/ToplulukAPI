using System;
namespace Topluluk.Shared.Messages.Authentication
{
    public class ResetPasswordCommand
    {
        public string To { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}

