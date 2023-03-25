using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Services.Interface;

public interface IChatService
{
    Task<Response<string>> SendMessage(string userId, MessageCreateDto message);
    Task<Response<string>> GetMessages(string? from, string? communityId, string? roomId);
    Task<Response<List<string>>> GetLatestMessages();
}