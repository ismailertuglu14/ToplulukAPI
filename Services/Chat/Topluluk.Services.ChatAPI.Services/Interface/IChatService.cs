using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Services.Interface;

public interface IChatService
{
    Task<Response<NoContent>> SendMessage(string userId, MessageCreateDto messageDto);
}