using AutoMapper;
using Topluluk.Services.ChatAPI.Model.Dto;
using Topluluk.Services.ChatAPI.Model.Entity;

namespace Topluluk.Services.ChatAPI.Model.Mapper;

public class GeneralMapper : Profile
{
    public GeneralMapper()
    {
        CreateMap<MessageCreateDto, Message>();
    }
}