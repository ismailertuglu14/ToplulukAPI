using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Topluluk.Services.MailAPI.Model.Dtos;
using Topluluk.Services.MailAPI.Model.Dtos.Event;
using Topluluk.Services.MailAPI.Services.Interface;

namespace Topluluk.Services.MailAPI.Services.Implementation;

public class MailService : IMailService
{
    private readonly IConfiguration _configuration;
    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendMailAsync(List<string> tos, String subject, String body)
    {
        try
        {
            MailMessage mail = new();
            foreach (var to in tos)
            {
                mail.To.Add(new MailAddress(to));
            }
            mail.From = new MailAddress("toplulukapp@gmail.com", "Topluluk");

            mail.Subject = subject;
            mail.Body = body;

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_configuration["Mail:Email"], _configuration["Mail:Password"]);
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }
        catch (Exception e)
        {

        }
    }

    public async Task SendRegisteredMail(MailDto mailDto)
    {
        try
        {

            string subject = "Topluluğa Hoş Geldiniz!";
            string body = "<html><body><h1>Topluluk</h1><p>Merhaba {0}," +
                          "</p>" +
                          "<p>Topluluğumuza katıldığınız için teşekkür ederiz!" +
                          " Burada diğer üyelerle etkileşimde bulunabilir, yeni arkadaşlar edinebilir ve ilginizi çeken konular" +
                          " hakkında tartışabilirsiniz.</p>" +
                          "<p>Eğer bir yanlışlık olduğunu düşünüyorsanız, lütfen bize bildirin.</p>" +
                          "<p>Sizinle birlikte topluluğumuz daha da büyüyecek. Umarız hoş vakit geçirirsiniz!</p>" +
                          "<p>İyi günler dileriz,<br/>Topluluk Ekibi</p>" +
                          "<p><a href='mailto:iletisim@topluluk.com'>İletişime Geç</a></p></body></html>";
            body = String.Format(body, mailDto.FullName);
            await SendMailAsync(new List<string>() { mailDto.To }, subject, body);
        }
        catch (Exception e)
        {

        }
    }

    public async Task SendResetPasswordMail(ResetPasswordDto resetDto)
    {
        try
        {
            var subject = "Şifre Sıfırlama İsteği";
            string body = string.Format(@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <title>Şifre Sıfırlama Maili</title>
                        </head>
                        <body>
                            <h1>Şifrenizi sıfırlayabilirsiniz!</h1>
                            <p>Merhaba,</p>
                            <p>Şifrenizi sıfırlamak için gereken kod aşağıda verilmiştir. Lütfen bu kodu kimseyle paylaşmayınız!</p>
                            <p>{0}</p>
                            <p>Eğer bu mesajı yanlışlıkla aldıysanız, lütfen dikkate almayınız.</p>
                        </body>
                    </html>
                    ", resetDto.Code);
            await SendMailAsync(new List<string>() { resetDto.To }, subject, body);
        }
        catch (Exception e)
        {

        }
    }

    public async Task EventDeletedMail(EventDeletedDto dto)
    {
        try
        {
            var subject = $"{dto.EventName} iptal oldu.";
            for (int i = 0; i < dto.UserMails.Count; i++)
            {
                string body = string.Format(@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <title>Merhaba {0}</title>
                        </head>
                        <body>
                            <h3>{1} adlı etkinliğin iptal olduğunu bildirmek isteriz.</h3>
                            <p>İyi günler dileriz.</p>
                        </body>
                    </html>
                    ",dto.UserNames[i], dto.EventName);
                await SendMailAsync(new List<string>(){dto.UserMails[i]}, subject, body);
            }
        }
        catch (Exception e)
        {

        }
    }
}