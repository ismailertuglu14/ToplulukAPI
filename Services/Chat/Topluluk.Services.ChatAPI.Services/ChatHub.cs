using Microsoft.AspNetCore.SignalR;
using Topluluk.Services.ChatAPI.Model.Dto;

namespace Topluluk.Services.ChatAPI.Services;

public class ChatHub : Hub
{
    public async Task GetUserName(string userName)
    {
        Client client = new()
        {
            ConnectionId = Context.ConnectionId,
            UserName = userName
        };
    }

    public async Task SendMessageAsync(string to, string message)
    {
        await Clients.Client(to).SendAsync("receiveMessage",message);
    }

    public override  async Task OnConnectedAsync()
    { 
        await Clients.Caller.SendAsync("getConnectionId", Context.ConnectionId);
    }
}