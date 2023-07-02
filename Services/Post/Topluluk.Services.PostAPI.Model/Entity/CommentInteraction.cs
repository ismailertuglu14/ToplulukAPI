using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity;

public class CommentInteraction : AbstractEntity
{
    public string UserId { get; set; }
    public string CommentId { get; set; }
    public CommentInteractionType Type { get; set; }
    
}
public enum CommentInteractionType
{
    DISLIKE = 0,
    LIKE = 1
}


