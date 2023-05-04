using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto;

public class PostInteractionCreateDto
{ 
    public InteractionEnum InteractionType { get; set; }
}