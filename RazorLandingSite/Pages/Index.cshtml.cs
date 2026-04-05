using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace Greenswamp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostSubscribeAsync(string email)
    {
        try
        {
            _logger.LogInformation("=== SUBSCRIBE HANDLER CALLED ===");
            _logger.LogInformation("Email received: {Email}", email);
            
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Empty email received");
                return BadRequest("Email is required");
            }
            
            if (!IsValidEmail(email))
            {
                _logger.LogWarning("Invalid email format: {Email}", email);
                return BadRequest("Invalid email format");
            }

            // Создаем директорию data если её нет
            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            _logger.LogInformation("Data directory path: {DataDirectory}", dataDirectory);
            
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                _logger.LogInformation("Created data directory");
            }
            
            // Формируем имя файла
            var today = DateTime.Now.ToString("yyyyMMdd");
            var subscribeFile = Path.Combine(dataDirectory, $"subscribers_{today}.csv");
            _logger.LogInformation("Subscribe file path: {SubscribeFile}", subscribeFile);
            
            var fileExists = System.IO.File.Exists(subscribeFile);
            _logger.LogInformation("File exists: {FileExists}", fileExists);

            // Создаем запись
            var id = Guid.NewGuid().ToString();
            var subscribedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Записываем в файл
            using (var writer = new StreamWriter(subscribeFile, append: true))
            {
                if (!fileExists)
                {
                    // Записываем заголовки
                    await writer.WriteLineAsync("Id,Email,SubscribedAt,IpAddress");
                    _logger.LogInformation("Created new CSV file with headers");
                }
                // Записываем данные
                await writer.WriteLineAsync($"{id},{email},{subscribedAt},{ipAddress}");
            }

            _logger.LogInformation("Successfully saved subscriber: {Email}", email);
            
            // Проверим, что файл создался
            if (System.IO.File.Exists(subscribeFile))
            {
                var fileContent = await System.IO.File.ReadAllTextAsync(subscribeFile);
                _logger.LogInformation("File content: {FileContent}", fileContent);
            }
            
            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving subscriber {Email}", email);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}