namespace Topluluk.Services.AuthenticationAPI.Model.Dto;

public class PasswordChangeDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}