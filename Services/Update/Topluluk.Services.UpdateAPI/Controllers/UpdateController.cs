using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.UpdateAPI.Model.Dtos;
using Topluluk.Services.UpdateAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.UpdateAPI.Controllers;

public class UpdateController : BaseController
{
    private readonly IUpdateService _updateService;

    public UpdateController(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    [HttpPost("check-force-update")]
    public async Task<Response<GetForceUpdateDto>> CheckForceUpdate(CurrentVersion version)
    {
        return await _updateService.CheckForceUpdate(version);
    }
    
    [HttpPost("publish-new-version")]
    public async Task<Response<NoContent>> PublishNewVersion(PublishNewVersionDto dto)
    {
        return await _updateService.PublishNewVersion(this.Roles, dto);
    }
}