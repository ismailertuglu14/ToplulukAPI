using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Controllers;

public class ChatController : BaseController
{

    private readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("send")]
    public async Task<Response<string>> SendMessage(MessageCreateDto message)
    {
        return await _chatService.SendMessage(this.UserId, message);
    }
    
}