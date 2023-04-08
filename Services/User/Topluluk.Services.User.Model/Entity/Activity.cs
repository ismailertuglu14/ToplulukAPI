using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Model.Entity;

public class Activity : AbstractEntity
{
    
}

public enum ActivityEnums
{
    JOINED_COMMUNITY = 0,
    CREATED_COMMUNITY = 1,
    GOING_EVENT = 2,
}