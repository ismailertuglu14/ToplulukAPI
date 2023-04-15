using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Model.Entity;

public class CommunityParticipiant : AbstractEntity
{
    public string UserId { get; set; }
    public string CommunityId { get; set; }
}