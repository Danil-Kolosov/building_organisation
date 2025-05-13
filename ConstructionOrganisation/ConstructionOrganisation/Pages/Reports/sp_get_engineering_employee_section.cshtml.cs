using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_engineering_employee_section : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_engineering_employee_section(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int SectionNumber { get; set; }

        public List<EngineeringEmployeeReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<EngineeringEmployeeReport>(
                        "CALL sp_get_engineering_employee_section({0})",
                        SectionNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class EngineeringEmployeeReport
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime HireDate { get; set; }
            public string Position { get; set; }
        }
    }
}