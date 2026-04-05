using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Greenswamp.Services;

namespace Greenswamp.Pages;

public class ContactModel : PageModel
{
    private readonly CsvService _csvService;
    private readonly ILogger<ContactModel> _logger;

    public ContactModel(CsvService csvService, ILogger<ContactModel> logger)
    {
        _csvService = csvService;
        _logger = logger;
    }

    [BindProperty]
    public ContactInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Серверная валидация
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for contact form");
            return Page();
        }

        // Дополнительная проверка email на домен .edu
        if (!Input.Email.EndsWith(".edu", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("Input.Email", "Только email адреса с доменом .edu разрешены");
            return Page();
        }

        try
        {
            // Сохраняем сообщение в CSV
            await _csvService.SaveContactMessageAsync(
                Input.Name,
                Input.Email,
                Input.Subject,
                Input.Message
            );

            _logger.LogInformation("Contact message saved from {Email}", Input.Email);
            
            TempData["Success"] = true;
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving contact message from {Email}", Input.Email);
            ModelState.AddModelError("", "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.");
            return Page();
        }
    }
}

public class ContactInputModel
{
    [Required(ErrorMessage = "Пожалуйста, укажите ваше имя")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
    [Display(Name = "Ваше имя")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Пожалуйста, укажите ваш email")]
    [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный email адрес")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Пожалуйста, выберите тему сообщения")]
    [Display(Name = "Тема")]
    public string Subject { get; set; } = "";

    [Required(ErrorMessage = "Пожалуйста, введите текст сообщения")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Сообщение должно содержать от 10 до 2000 символов")]
    [Display(Name = "Сообщение")]
    public string Message { get; set; } = "";
}