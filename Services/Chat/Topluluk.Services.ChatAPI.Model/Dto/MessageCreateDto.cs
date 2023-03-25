namespace Topluluk.Services.ChatAPI.Model.Dto;

public class MessageCreateDto
{
    public string Message { get; set; }
    public string? To { get; set; }
    public string? GroupId { get; set; }
    public string? CommunityId { get; set; }
    public string? RoomId { get; set; }
}