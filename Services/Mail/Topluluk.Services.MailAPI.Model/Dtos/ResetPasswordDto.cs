namespace Topluluk.Services.MailAPI.Model.Dtos;

public class ResetPasswordDto
{
    public string To { get; set; }
    public string UserId { get; set; }
    public string ResetToken { get; set; }
}