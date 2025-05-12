using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Pages.Reports
{
    public class EngineeringEmployeesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EngineeringEmployeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ManagementId { get; set; } = 1;

        public List<EmployeeReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.SectionEmployees
    .Include(se => se.SectionName)
        .ThenInclude(s => s.ManagementNumber) // Если нужно обращаться к Management
    .Include(se => se.EmployeeCodeNavigation)
        .ThenInclude(e => e.GroupName)
    .Where(se => se.SectionName.ManagementNumber == ManagementId &&
                se.EmployeeCodeNavigation.GroupName.GroupName.Contains("инженер"))
    .Select(se => new EmployeeReport
    {
        SectionNameID = se.SectionNameId,
        FirstName = se.EmployeeCodeNavigation.FirstName,
        LastName = se.EmployeeCodeNavigation.LastName,
        Position = se.EmployeeCodeNavigation.GroupName.GroupName
    })
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
            public int SectionNameID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime HireDate { get; set; }
            public string Position { get; set; }
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