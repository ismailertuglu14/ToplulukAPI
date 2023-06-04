using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Dto;

public class MessageCreateDto
{
    public string ReceiverId { get; set; }
    public string Content { get; set; }
}