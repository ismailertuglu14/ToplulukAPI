using AutoMapper;
using Topluluk.Services.ChatAPI.Data.Interface;
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

    
}