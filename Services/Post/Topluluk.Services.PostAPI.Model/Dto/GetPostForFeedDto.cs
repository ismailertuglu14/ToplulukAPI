namespace Topluluk.Services.PostAPI.Model.Dto;

public class GetPostForFeedDto
{
    public string Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsFollowing { get; set; }

    public string Description { get; set; }
    public List<string>? PostImages { get; set; }

    public CommunityLink? Community { get; set; }
    public EventLink? Event { get; set; }

    public int CommentCount { get; set; }
    public int InteractionCount { get; set; }


    public GetPostForFeedDto()
    {
        PostImages = new List<string>();
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