using Topluluk.Shared.Enums;

namespace Topluluk.Services.CommunityAPI.Model.Dto.Http;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfileImage { get; set; }
    public GenderEnum Gender { get; set; }
}