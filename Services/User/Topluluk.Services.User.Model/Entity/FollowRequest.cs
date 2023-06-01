using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Model.Entity;

public class FollowRequest : AbstractEntity
{
  

    // ID of the person making the request
    public string SourceId { get; set; }
    
    
    // Id of the requested person
    public string TargetId { get; set; }
    
    
    public FollowRequest(string sourceId, string targetId)
    {
        SourceId = sourceId;
        TargetId = targetId;
    }
}