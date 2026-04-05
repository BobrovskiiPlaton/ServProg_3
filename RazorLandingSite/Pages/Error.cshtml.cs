using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Greenswamp.Pages;

public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int? code = null)
    {
        var statusCode = code ?? 404;
        _logger.LogWarning("Страница не найдена: {Path}, Status Code: {StatusCode}", 
            Request.Path, statusCode);
        
        Response.StatusCode = statusCode;
    }
}