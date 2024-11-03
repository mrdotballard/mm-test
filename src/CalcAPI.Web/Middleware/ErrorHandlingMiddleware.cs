using Microsoft.AspNetCore.Authentication;

namespace CalcAPI.Web.Middleware;

/// <summary>
/// Middleware to handle both authentication failures and unhandled exceptions globally
/// </summary>
public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check authentication first
            var authResult = await context.AuthenticateAsync();
            if (!authResult.Succeeded)
            {
                await HandleAuthenticationFailureAsync(context, authResult.Failure?.Message);
                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleAuthenticationFailureAsync(HttpContext context, string? message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var response = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = message ?? "Unauthorized",
            Timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ArgumentException argEx:
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = argEx.Message;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case InvalidOperationException invEx:
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = invEx.Message;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case UnauthorizedAccessException uaEx:
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = uaEx.Message;
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                break;

            default:
                // Don't expose internal error details in production
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An unexpected error occurred";
#if DEBUG
                response.Details = exception.ToString();
#endif
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Standardized API error response model
/// </summary>
public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Extension method to easily add the middleware to the application pipeline
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}