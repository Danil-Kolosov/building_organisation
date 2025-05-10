using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConstructionOrganisation.Data;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Pages
{
    [Authorize] // Только для авторизованных пользователей
    public class ReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // Логика загрузки данных при GET-запросе
        }
    }
}