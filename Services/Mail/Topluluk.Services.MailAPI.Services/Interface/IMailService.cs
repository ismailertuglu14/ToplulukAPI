using Topluluk.Services.MailAPI.Model.Dtos;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.MailAPI.Services.Interface;

public interface IMailService
{
    Task SendMailAsync(List<string> tos, String subject, String body);
    Task SendRegisteredMail(MailDto mailDto);
    Task SendResetPasswordMail(ResetPasswordDto resetDto);
}