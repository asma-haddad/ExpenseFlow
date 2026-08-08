using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Infrastructure.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace ExpenseFlow.Api.Middleware;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;

    public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var shouldSkipAudit =
                context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/scalar");

            if (!shouldSkipAudit)
            {
                try
                {
                    db.AuditLog.Add(new AuditLog
                    {
                        Method = context.Request.Method,
                        Path = context.Request.Path,
                        StatusCode = context.Response.StatusCode,
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                        UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    });

                    await db.SaveChangesAsync(context.RequestAborted);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save audit log.");
                }
            }
        }
    }
}