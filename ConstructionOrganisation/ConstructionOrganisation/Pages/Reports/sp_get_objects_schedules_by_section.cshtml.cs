using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_objects_schedules_by_section : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_objects_schedules_by_section(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int SectionID { get; set; }

        public List<ObjectScheduleReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<ObjectScheduleReport>(
                        "CALL sp_get_objects_schedules_by_section({0})",
                        SectionID)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class ObjectScheduleReport
        {
            public int ManagementNumber { get; set; }
            public string SectionName { get; set; }
            public int ObjectNameID { get; set; }
            public string ObjectName { get; set; }
            public int WorkNumber { get; set; }
            public string WorkTypeName { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
        }
    }
}