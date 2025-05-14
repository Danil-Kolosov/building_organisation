using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_management_machines : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_management_machines(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ManagementNumber { get; set; }

        public List<ManagementMachineReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<ManagementMachineReport>(
                        "CALL sp_get_management_machines({0})",
                        ManagementNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class ManagementMachineReport
        {
            public int SerialNumber { get; set; }
            public string MachineType { get; set; }
        }
    }
}