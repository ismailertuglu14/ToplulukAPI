using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity;

public class Conversation : AbstractEntity
{
    public List<string> Participiants { get; set; }
    
    public Conversation()
    {
        Participiants = new List<string>();
    }
}

public enum ConversationType
{
    PRIVATE,
    GROUP,
    CHANNEL
}