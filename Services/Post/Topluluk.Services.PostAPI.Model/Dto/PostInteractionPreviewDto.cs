using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto;

public class PostInteractionPreviewDto
{
    public InteractionEnum Interaction { get; set; }
    public int InteractionCount { get; set; }
}