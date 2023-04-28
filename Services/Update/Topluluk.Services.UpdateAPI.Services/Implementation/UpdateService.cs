using AutoMapper;
using Topluluk.Services.UpdateAPI.Data.Interface;
using Topluluk.Services.UpdateAPI.Model.Dtos;
using Topluluk.Services.UpdateAPI.Model.Entity;
using Topluluk.Services.UpdateAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.UpdateAPI.Services.Implementation;

public class UpdateService : IUpdateService
{
    private readonly IUpdateRepository _updateRepository;
    private readonly IMapper _mapper;
    public UpdateService(IUpdateRepository updateRepository, IMapper mapper)
    {
        _updateRepository = updateRepository;
        _mapper = mapper;
    }

    public async Task<Response<NoContent>> PublishNewVersion(List<string> roles, PublishNewVersionDto dto)
    {
        try
        {
            if (roles.Count == 0 || !roles.Contains(UserRoles.ADMIN))
            {
                return await Task.FromResult(Response<NoContent>.Fail("UnAuthorized", ResponseStatus.Unauthorized));
            }

            AppVersion appVersion = _mapper.Map<AppVersion>(dto);
            await _updateRepository.InsertAsync(appVersion);
            return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred: {e}",
                ResponseStatus.InitialError));
        }
    }

    public async Task<Response<GetForceUpdateDto>> CheckForceUpdate(CurrentVersion version)
    {
        try
        {
            var isRequired = await _updateRepository.CheckIfUpdateIsRequired(version.Version);
            return await Task.FromResult(Response<GetForceUpdateDto>.Success(new() { IsRequired = isRequired }, ResponseStatus.Success));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<GetForceUpdateDto>.Fail($"Some error occurred: {e}",
                ResponseStatus.InitialError));
        }
    }
}