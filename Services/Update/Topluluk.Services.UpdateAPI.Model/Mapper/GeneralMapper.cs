using AutoMapper;
using Topluluk.Services.UpdateAPI.Model.Dtos;
using Topluluk.Services.UpdateAPI.Model.Entity;

namespace Topluluk.Services.UpdateAPI.Model.Mapper;

public class GeneralMapper : Profile
{
    public GeneralMapper()
    {
        CreateMap<AppVersion, GetForceUpdateDto>();
        CreateMap<PublishNewVersionDto, AppVersion>();
    }
}