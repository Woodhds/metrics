using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace metrics.Pages.Account
{
    public class Register : PageModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Пароли должны совпадать")]
        public string ConfirmPassword { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}