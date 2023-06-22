using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Exceptions;

namespace Topluluk.Shared.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ResponseStatus statusCode = exception switch
        {
            NotFoundException => ResponseStatus.NotFound,
            UnauthorizedAccessException => ResponseStatus.Unauthorized,
            ArgumentException => ResponseStatus.BadRequest,
            _ => ResponseStatus.InitialError
        };

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var errorResponse = new Response<string>
        {
            Data = null!,
            StatusCode = statusCode,
            IsSuccess = false,
            Errors = new List<string> { exception.Message, exception.ToString() }
        };

        var json = JsonConvert.SerializeObject(errorResponse);
        
        return context.Response.WriteAsync(json);
    }

}