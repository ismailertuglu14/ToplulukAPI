using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Topluluk.Shared.Middleware;

public class RequestResponseMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestResponseMiddleware> logger;
    
    public RequestResponseMiddleware(RequestDelegate Next, ILogger<RequestResponseMiddleware> Logger)
    {
        next = Next;
        logger = Logger;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        // Request
        logger.LogInformation("Query Keys: {RequestQueryString}", httpContext.Request.QueryString);

        var originalResponseBody = httpContext.Response.Body; // Özgün Response Body'sini 
        var tempStream = new MemoryStream();
        httpContext.Response.Body = tempStream;


        
        await next.Invoke(httpContext); // Response bu satırda oluşuyor.
    
        // Response tamamlanmasını bekleyin
        await httpContext.Response.Body.FlushAsync();
    
        // Bellekteki yanıtı okuyun
        tempStream.Seek(0, SeekOrigin.Begin);
        String responseText = await new StreamReader(tempStream, Encoding.UTF8).ReadToEndAsync();

        // Geçici bellek akışını geri yükle
        tempStream.Seek(0, SeekOrigin.Begin);
        await tempStream.CopyToAsync(originalResponseBody);
        httpContext.Response.Body = originalResponseBody;
    }

}