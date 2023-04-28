using Topluluk.Services.UpdateAPI.Model.Dtos;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.UpdateAPI.Services.Interface;

public interface IUpdateService
{
    Task<Response<NoContent>> PublishNewVersion(List<string> roles, PublishNewVersionDto dto);
    Task<Response<GetForceUpdateDto>> CheckForceUpdate(CurrentVersion version);
}