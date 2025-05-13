using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionOrganisation.Pages.Reports;
public class IndexModel : PageModel
{
    public List<string> AvailableTables { get; } = new()
    {
        "Employee",
        "Projects",
        "Contracts"
        // Добавьте все таблицы
    };

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            return RedirectToPage("/Account/Login");

        return Page();
    }
}