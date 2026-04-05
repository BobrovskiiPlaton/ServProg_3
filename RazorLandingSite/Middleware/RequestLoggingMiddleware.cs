using System.Text;
using Serilog;

namespace Greenswamp.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var method = request.Method;
        var path = request.Path;
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = request.Headers["User-Agent"].ToString();

        // Логируем запрос
        Log.Information("[{Timestamp}] {Method} {Path} | IP: {IP} | UA: {UserAgent}", 
            timestamp, method, path, ip, userAgent.Length > 50 ? userAgent[..50] : userAgent);

        // Засекаем время выполнения
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        
        // Логируем ответ
        Log.Information("[{Timestamp}] {Method} {Path} -> Status: {StatusCode} | Duration: {Duration}ms",
            timestamp, method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}