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
    public class sp_work_type_late_section : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_work_type_late_section(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int SectionId { get; set; }

        public List<WorkTypeLateReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<WorkTypeLateReport>(
                        "CALL sp_work_type_late_section({0})",
                        SectionId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class WorkTypeLateReport
        {
            public int WorkTypeID { get; set; }
            public string WorkTypeName { get; set; }
            public int LateCount { get; set; }
            public double AvgDelayDays { get; set; }
            public int MaxDelayDays { get; set; }
        }
    }
}