using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Services.Interface;

namespace Topluluk.Services.ChatAPI.Services.Hubs;

public class ChatHub : Hub
{
    private readonly static ConnectionMapping<string> _connections =
        new ConnectionMapping<string>();
    public string GetConnectionId() => Context.ConnectionId;

    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    public async Task SendMessageAsync(string to, MessageReceivedDto message)
    {
       if (_connections.GetConnections(to).Count() <= 0)
        {
            await Task.CompletedTask;
        }

        var targetUserConnectionId = _connections.GetConnections(to).Last();
        
        await Clients.Client(targetUserConnectionId).SendAsync("receiveMessage", message);
        
        await _chatService.SendMessage(message.From.Id,new MessageCreateDto
        {
            Content = message.Message,
            ReceiverId = to
        });
    }

    public async Task AssignConnectionId(string userId)
    {
        _connections.Add(userId, Context.ConnectionId);
    }
    
}