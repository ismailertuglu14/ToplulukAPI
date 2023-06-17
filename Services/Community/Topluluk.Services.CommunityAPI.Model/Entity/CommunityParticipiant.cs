using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Model.Entity;

public class CommunityParticipiant : AbstractEntity
{
    public string UserId { get; set; }
    public string CommunityId { get; set; }
    public bool IsShownOnProfile { get; set; }
    
    public ParticipiantStatus Status { get; set; }

    public CommunityParticipiant()
    {
        IsShownOnProfile = true;
    }
}

public enum ParticipiantStatus
{
    REQUESTED = 0,
    ACCEPTED = 1,
    REJECTED = 2
}