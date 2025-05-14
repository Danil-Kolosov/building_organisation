using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_machine_by_object : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_machine_by_object(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ObjectNumber { get; set; }

        public List<MachineByObjectReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<MachineByObjectReport>(
                        "CALL sp_get_machine_by_object({0})",
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

        public class MachineByObjectReport
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
