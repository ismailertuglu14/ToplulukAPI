namespace Topluluk.Services.AuthenticationAPI.Model.Dto;

public class ResetPasswordCheckOTPDto
{
    public string Mail { get; set; }
    public string Code { get; set; }
}