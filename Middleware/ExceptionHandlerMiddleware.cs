using System.Net;
using System.Text.Json;
using MovieReservationSystem.Backend.DTOs;

namespace MovieReservationSystem.Backend.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var details = env.IsDevelopment()
                ? new { type = ex.GetType().Name, stackTrace = ex.StackTrace }
                : null;
            var payload = JsonSerializer.Serialize(new ErrorResponse(ex.Message, details));
            await context.Response.WriteAsync(payload);
        }
    }
}
