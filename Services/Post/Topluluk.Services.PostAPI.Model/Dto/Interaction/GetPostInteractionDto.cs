using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Dto;

public class GetPostInteractionDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfileImage { get; set; }
    public GenderEnum Gender { get; set; }
}