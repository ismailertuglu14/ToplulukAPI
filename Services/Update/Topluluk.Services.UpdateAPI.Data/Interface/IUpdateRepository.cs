using DBHelper.Repository;
using Topluluk.Services.UpdateAPI.Model.Entity;

namespace Topluluk.Services.UpdateAPI.Data.Interface;

public interface IUpdateRepository : IGenericRepository<AppVersion>
{
    Task<AppVersion> GetLatestVersion();
    Task<bool> CheckIfUpdateIsRequired(double currentVersion);
}