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
    public class sp_material_late_all : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_material_late_all(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MaterialLateAllReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<MaterialLateAllReport>("CALL sp_material_late_all()")
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class MaterialLateAllReport
        {
            public int MaterialID { get; set; }
            public string MaterialName { get; set; }
            public int ManagementNumber { get; set; }
            public string SectionName { get; set; }
            public decimal PlannedQuantity { get; set; }
            public decimal RealQuantity { get; set; }
            public decimal Difference { get; set; }
            public int Count { get; set; }
        }
    }
}