namespace Topluluk.Services.MailAPI.Model.Dtos.Event;

public class EventDeletedDto
{
    public string EventName { get; set; }
    public List<string> UserMails { get; set; }
    public List<string> UserNames { get; set; }
}