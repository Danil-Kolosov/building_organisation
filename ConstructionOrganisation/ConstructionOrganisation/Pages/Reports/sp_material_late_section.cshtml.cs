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
    public class sp_material_late_section : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_material_late_section(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SectionName { get; set; }

        public List<MaterialLateReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<MaterialLateReport>(
                        "CALL sp_material_late_section({0})",
                        SectionName)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class MaterialLateReport
        {
            public int MaterialID { get; set; }
            public string MaterialName { get; set; }
            public string MeasurementUnits { get; set; }
            public decimal PlannedQuantity { get; set; }
            public decimal RealQuantity { get; set; }
            public decimal Difference { get; set; }
            public int Count { get; set; }
        }
    }
}