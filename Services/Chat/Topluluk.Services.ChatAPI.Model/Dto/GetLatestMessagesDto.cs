namespace Topluluk.Services.ChatAPI.Model.Dto;

public class GetLatestMessagesDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public ChatType ChatType { get; set; }
}

public enum ChatType
{
    PERSON = 0,
    GROUP = 1,
}