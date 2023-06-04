using AutoMapper;
using Topluluk.Services.ChatAPI.Data.Interface;
using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Model.Entity;
using Topluluk.Services.ChatAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.ChatAPI.Services.Implementation;

public class ChatService : IChatService
{
    private readonly IChatRepository _messageRepository;
   // private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;

    public ChatService(IChatRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
      //  _conversationRepository = conversationRepository;
    }


    public async Task<Response<NoContent>> SendMessage(string userId, MessageCreateDto messageDto)
    {
        var message = _mapper.Map<Message>(messageDto);
        message.SenderId = userId;
        
        await _messageRepository.InsertAsync(message);
        
        return Response<NoContent>.Success(ResponseStatus.Success);
    }
}