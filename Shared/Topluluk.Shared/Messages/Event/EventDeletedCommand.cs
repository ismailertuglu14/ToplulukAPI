namespace Topluluk.Shared.Messages.Event;

public class EventDeletedCommand
{
    public string EventName { get; set; }
    public List<string> UserMails { get; set; }
    public List<string> UserNames { get; set; }
}