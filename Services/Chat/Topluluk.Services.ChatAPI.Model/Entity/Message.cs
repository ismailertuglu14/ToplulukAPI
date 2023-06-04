using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity;

public class Message : AbstractEntity
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }

    public bool IsUpdated { get; set; }
    public List<string> ModifiedMessages { get; set; }
    public Message()
    {
        IsUpdated = false; 
    }
}