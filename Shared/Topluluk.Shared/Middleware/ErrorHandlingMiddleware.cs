using Microsoft.AspNetCore.Http;

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
            // Log the exception
            Console.WriteLine(ex.Message);

            // Return a 500 response to the client
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An error occurred.");
        }
    }
}