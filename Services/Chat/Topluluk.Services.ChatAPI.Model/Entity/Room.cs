using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity;

public class Room : AbstractEntity
{
    public string CreatedBy { get; set; }
    public string Name { get; set; }
    public RoomType RoomType { get; set; }
}

public enum RoomType
{
    TEXT = 0,
    VOICE = 1,
}