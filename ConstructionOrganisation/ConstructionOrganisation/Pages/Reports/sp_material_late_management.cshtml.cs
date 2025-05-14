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
    public class sp_material_late_management : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_material_late_management(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ManagementNum { get; set; } = 1;

        public List<MaterialLateManagementReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<MaterialLateManagementReport>(
                        "CALL sp_material_late_management({0})",
                        ManagementNum)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class MaterialLateManagementReport
        {
            public int MaterialID { get; set; }
            public string MaterialName { get; set; }
            public string SectionName { get; set; }
            public decimal PlannedQuantity { get; set; }
            public decimal RealQuantity { get; set; }
            public decimal Excess { get; set; }
            public int Count { get; set; }
        }
    }
}