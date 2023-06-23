using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto;

public class GetPostForFeedDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public GenderEnum Gender { get; set; }
    public bool IsFollowing { get; set; }

    public string Description { get; set; }
    public List<FileModel>? Files  { get; set; }

    public CommunityLink? Community { get; set; }
    public EventLink? Event { get; set; }
    public List<PostInteractionPreviewDto> InteractionPreviews { get; set; }
    public PostInteractedDto IsInteracted { get; set; }
    public bool IsSaved { get; set; } = false;
    

    public int CommentCount { get; set; }
    public int InteractionCount { get; set; }


    public GetPostForFeedDto()
    {
        Files = new List<FileModel>();
        InteractionPreviews = new List<PostInteractionPreviewDto>();
    }
}

public class CommunityLink
{
    public string? Id { get; set; }
    public string Title { get; set; }
    public string? CoverImage { get; set; }
}

public class EventLink
{
    public string? Id { get; set; }
    public string Title { get; set; }
    public string? CoverImage { get; set; }
}