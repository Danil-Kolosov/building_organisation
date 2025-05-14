using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_machine_by_object_by_date : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_machine_by_object_by_date(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ObjectNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime FirstDate { get; set; } = DateTime.Today.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime SecondDate { get; set; } = DateTime.Today;

        public List<MachineByObjectDateReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<MachineByObjectDateReport>(
                        "CALL sp_get_machine_by_object_by_date({0}, {1}, {2})",
                        ObjectNumber,
                        FirstDate.ToString("yyyy-MM-dd"),
                        SecondDate.ToString("yyyy-MM-dd"))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class MachineByObjectDateReport
        {
            public int SerialNumber { get; set; }
            public string MachineType { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
            public DateTime? RealStartDate { get; set; }
            public DateTime? RealEndDate { get; set; }
        }
    }
}