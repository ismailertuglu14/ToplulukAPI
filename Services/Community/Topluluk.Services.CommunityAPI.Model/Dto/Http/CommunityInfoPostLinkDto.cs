namespace Topluluk.Services.CommunityAPI.Model.Dto.Http;

// We use this dto for select community in dropdown.
public class CommunityInfoPostLinkDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? CoverImage { get; set; }
}