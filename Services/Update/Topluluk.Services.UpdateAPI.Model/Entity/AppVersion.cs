using Topluluk.Shared.Dtos;

namespace Topluluk.Services.UpdateAPI.Model.Entity;

public class AppVersion : AbstractEntity
{
    public double Version { get; set; }
    public bool IsRequired { get; set; }
}