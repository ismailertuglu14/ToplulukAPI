using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Topluluk.Services.ChatAPI.Data.Interface;
using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Model.Entity;
using Topluluk.Services.ChatAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.ChatAPI.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public ChatService(IChatRepository chatRepository, IMapper mapper)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
    }

    public async Task<Response<string>> SendMessage(string userId,MessageCreateDto message)
    {
        try
        {
            if (userId.IsNullOrEmpty()) throw new Exception("User not found");

            if (userId.Equals(message.To)) throw new Exception("Cant send message yourself");

            if (!message.To.IsNullOrEmpty() && !message.CommunityId.IsNullOrEmpty())
                throw new Exception("You cant send message different places at same time!");

            if (!message.CommunityId.IsNullOrEmpty() && message.RoomId.IsNullOrEmpty())
                throw new Exception(
                    "You cant send message outside of the room. Please provide a roomId and try again!");
            if (!message.To.IsNullOrEmpty() && !message.RoomId.IsNullOrEmpty())
                throw new Exception("You cant send message different places at same time!");

            Message _message = _mapper.Map<Message>(message);
            _message.From = userId;

            await _chatRepository.InsertAsync(_message);
            return await Task.FromResult(Response<string>.Success("", ResponseStatus.Success));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<string>.Fail($"Some error occured {e}", ResponseStatus.InitialError));
        }
       
    }

    public Task<Response<string>> GetMessages(string? from, string? communityId, string? roomId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<string>>> GetLatestMessages()
    {
        throw new NotImplementedException();
    }
}