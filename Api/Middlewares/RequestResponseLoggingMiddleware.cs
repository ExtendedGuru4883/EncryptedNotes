using Shared.Helpers;

namespace EncryptedNotes.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Handling request: {method} to {url}", SanitizeForLogging.Sanitize(context.Request.Method),
            SanitizeForLogging.Sanitize(context.Request.Path));

        await _next(context);

        _logger.LogInformation("Response: {statusCode}", context.Response.StatusCode);
    }
}