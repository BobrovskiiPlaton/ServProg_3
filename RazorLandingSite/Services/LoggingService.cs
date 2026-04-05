using System.Text;

namespace Greenswamp.Services;

public class LoggingService
{
    private readonly ILogger<LoggingService> _logger;
    private readonly string _logDirectory;
    private readonly object _lock = new object();

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
        _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    public async Task LogRequestAsync(HttpContext context)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
        logEntry.AppendLine($"Method: {context.Request.Method}");
        logEntry.AppendLine($"Path: {context.Request.Path}");
        logEntry.AppendLine($"IP: {context.Connection.RemoteIpAddress}");
        logEntry.AppendLine($"User-Agent: {context.Request.Headers["User-Agent"]}");
        logEntry.AppendLine($"Status Code: {context.Response.StatusCode}");
        logEntry.AppendLine(new string('-', 50));

        var logFile = Path.Combine(_logDirectory, $"requests_{DateTime.Now:yyyyMMdd}.log");
        
        lock (_lock)
        {
            File.AppendAllText(logFile, logEntry.ToString());
        }
        
        await Task.CompletedTask;
    }
}