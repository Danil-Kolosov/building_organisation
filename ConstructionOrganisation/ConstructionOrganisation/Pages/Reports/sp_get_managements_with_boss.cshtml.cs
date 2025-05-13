using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_managements_with_boss : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_managements_with_boss(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]

        public List<EmployeeReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // Используем SqlQueryRaw для вызова хранимой процедуры
                ReportData = await _context.Database
                    .SqlQueryRaw<EmployeeReport>(
                        "CALL sp_get_managements_with_boss()")
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData["Error"] для отображения на странице
            }
        }

        public class EmployeeReport
        {
            public int ManagementNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime HireDate { get; set; }
            public string GroupName { get; set; }
        }
    }
}

//        public class EngineeringEmployeeReport
//        {
//            public int SectionNameID { get; set; }
//            public string FirstName { get; set; }
//            public string LastName { get; set; }
//            public string HireDate { get; set; }
//            public string Position { get; set; }
//        }
//    }
//}