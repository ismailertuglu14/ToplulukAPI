using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Model.Entity;

public class FollowRequest : AbstractEntity
{
    public string SourceId { get; set; }
    public string TargetId { get; set; }
}