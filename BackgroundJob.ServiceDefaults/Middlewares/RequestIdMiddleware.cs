using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestIdMiddleware
{
    private const string RequestIdHeader = "X-Request-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestIdMiddleware> _logger;

    public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;

        // Log the Request ID
        _logger.LogInformation("Processing request with TraceIdentifier: {TraceIdentifier}", requestId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[RequestIdHeader] = requestId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
