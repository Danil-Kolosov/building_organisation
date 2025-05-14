using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_work_type_late_management : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_work_type_late_management(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ManagementId { get; set; } = 1;

        public List<WorkTypeLateManagementReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<WorkTypeLateManagementReport>(
                        "CALL sp_work_type_late_management({0})",
                        ManagementId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class WorkTypeLateManagementReport
        {
            public int WorkTypeID { get; set; }
            public string WorkTypeName { get; set; }
            public string SectionName { get; set; }
            public int LateCount { get; set; }
            public double AvgDelayDays { get; set; }
        }
    }
}