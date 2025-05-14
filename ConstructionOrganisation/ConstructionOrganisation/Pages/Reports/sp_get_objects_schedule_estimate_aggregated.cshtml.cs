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
    public class sp_get_objects_schedule_estimate_aggregated : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_objects_schedule_estimate_aggregated(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ObjectNumber { get; set; }

        public List<ObjectScheduleEstimateReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<ObjectScheduleEstimateReport>(
                        "CALL sp_get_objects_schedule_estimate_aggregated({0})",
                        ObjectNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class ObjectScheduleEstimateReport
        {
            public int WorkNumber { get; set; }
            public string WorkTypeName { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
            public string Materials { get; set; }
            public decimal TotalCost { get; set; }
        }
    }
}