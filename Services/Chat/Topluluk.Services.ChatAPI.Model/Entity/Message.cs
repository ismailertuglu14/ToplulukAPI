using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity;

public class Message : AbstractEntity
{
    public string CreatedBy { get; set; }
    public string Content { get; set; }

}