using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorLandingSite.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public ContactMessage ContactMessage { get; set; }
        
        public bool ShowSuccessMessage { get; set; }

        public void OnGet()
        {
            ShowSuccessMessage = false;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Здесь можно добавить логику отправки email или сохранения в базу данных
            ShowSuccessMessage = true;
            ModelState.Clear();
            ContactMessage = new ContactMessage();
            
            return Page();
        }
    }

    public class ContactMessage
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Поле Тема обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "Тема не должна превышать 200 символов")]
        public string Subject { get; set; }
        
        [Required(ErrorMessage = "Поле Сообщение обязательно для заполнения")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Сообщение должно содержать от 10 до 1000 символов")]
        public string Message { get; set; }
    }
}