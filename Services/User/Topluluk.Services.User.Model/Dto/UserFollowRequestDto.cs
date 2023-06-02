using Topluluk.Shared.Enums;

namespace Topluluk.Services.User.Model.Dto;

// Use for Incoming Requests
public class UserFollowRequestDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string? ProfileImage { get; set; }
    public GenderEnum Gender { get; set; }
    public DateTime CreatedAt { get; set; }
}