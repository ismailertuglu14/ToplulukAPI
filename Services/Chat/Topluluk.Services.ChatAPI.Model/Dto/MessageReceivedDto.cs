namespace Topluluk.Services.ChatAPI.Model.Dto;

public class MessageReceivedDto
{
    public UserDto From { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}