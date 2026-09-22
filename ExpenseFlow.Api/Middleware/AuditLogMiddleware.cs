using ExpenseFlow.Domain.Model.AuditLog;
using ExpenseFlow.Infrastructure.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ExpenseFlow.Api.Middleware;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuditLogMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, AuditScope auditScope, AppDbContext dataContext)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/favicon.ico"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var executionTime = DateTime.UtcNow;
        var correlationId = context.TraceIdentifier;

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            context.Request.EnableBuffering();

            await _next(context);

            stopwatch.Stop();

            responseBody.Seek(0, SeekOrigin.Begin);

            string responseText = string.Empty;

            if (IsTextResponse(context.Response.ContentType))
            {
                using var responseReader = new StreamReader(
                    responseBody,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                responseText = await responseReader.ReadToEndAsync();

                responseBody.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                responseText = "[Binary response skipped]";
            }

            if (auditScope.Logs?.Any() == true)
            {
                foreach (var auditLog in auditScope.Logs)
                {
                    try
                    {
                        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                        var ipAddress = string.IsNullOrEmpty(forwardedHeader)
                            ? context.Connection.RemoteIpAddress?.ToString()
                            : forwardedHeader.Split(',')[0];

                        auditLog.CorrelationId = correlationId;
                        auditLog.ClientIpAddress = ipAddress;
                        auditLog.BrowserInfo = context.Request.Headers["User-Agent"].ToString();
                        auditLog.MachineName = Environment.MachineName;
                        auditLog.MachineVersion = Environment.Version.ToString();
                        auditLog.MachineOsVersion = Environment.OSVersion.ToString();

                        var routeData = context.GetRouteData();

                        auditLog.ServiceName = routeData?.Values["controller"]?.ToString();
                        auditLog.MethodName = routeData?.Values["action"]?.ToString();
                        auditLog.HttpMethod = context.Request.Method;

                        auditLog.RequestUrl =
                            $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

                        auditLog.ResponseStatus = context.Response.StatusCode.ToString();
                        auditLog.ExecutionTime = executionTime;
                        auditLog.ExecutionDuration = stopwatch.ElapsedMilliseconds.ToString();

                        auditLog.RequestHeaders = JsonConvert.SerializeObject(
                            context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

                        auditLog.QueryParameters = JsonConvert.SerializeObject(
                            context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

                        context.Request.Body.Position = 0;

                        using var reader = new StreamReader(
                            context.Request.Body,
                            Encoding.UTF8,
                            detectEncodingFromByteOrderMarks: false,
                            leaveOpen: true);

                        var requestBody = await reader.ReadToEndAsync();

                        context.Request.Body.Position = 0;

                        if (!string.IsNullOrEmpty(requestBody))
                        {
                            try
                            {
                                var parsedBody = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                                auditLog.BodyParameters = JsonConvert.SerializeObject(parsedBody);
                            }
                            catch
                            {
                                auditLog.BodyParameters = requestBody;
                            }
                        }

                        if (context.Response.StatusCode >= 400)
                        {
                            auditLog.Exception = responseText;
                        }

                        dataContext.AuditLog.Add(auditLog);
                        await dataContext.SaveChangesAuditLogAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to save audit log: {ex.Message}");
                    }
                }
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch
        {
            context.Response.Body = originalBodyStream;
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static bool IsTextResponse(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("text/", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("application/problem+json", StringComparison.OrdinalIgnoreCase);
    }
}