using System;
namespace Topluluk.Shared.Enums
{
    public enum ResponseStatus
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,

        Failed = 10000,

        UsernameInUse = 10001,
        EmailInUse = 10002,

        // 
        CommunityOwnerExist = 10300,

        NotAuthenticated = 10401,
        AccountLocked = 10402,

        InitialError = 500,

        SMSServiceError = 10501,
        EmailServiceError = 10502
    }
}

