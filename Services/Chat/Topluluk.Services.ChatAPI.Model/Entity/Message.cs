using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity;

public class Message : AbstractEntity
{
    public string message { get; set; }
    public string From { get; set; }
    public string? To { get; set; }
    public string? GroupId { get; set; }
    public string? CommunityId { get; set; }
    public string? RoomId { get; set; }
}