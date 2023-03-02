using System;
namespace Topluluk.Shared.Enums
{
    public enum ResponseStatus
    {
        Success = 200,
        BadRequest = 400,
        NotFound = 404,

        UsernameInUse = 10001,
        EmailInUse = 10002,
        NotAuthenticated = 10401,

        InitialError = 500,

        SMSServiceError = 101,
        EmailServiceError = 102
    }
}

