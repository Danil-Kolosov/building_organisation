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
    public class sp_work_type_late_all : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_work_type_late_all(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<WorkTypeLateAllReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<WorkTypeLateAllReport>("CALL sp_work_type_late_all()")
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class WorkTypeLateAllReport
        {
            public int WorkTypeID { get; set; }
            public string WorkTypeName { get; set; }
            public int ManagementNumber { get; set; }
            public string SectionName { get; set; }
            public int LateCount { get; set; }
            public double AvgDelayDays { get; set; }
        }
    }
}