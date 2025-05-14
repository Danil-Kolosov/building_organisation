using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_object_report : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_object_report(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ObjectNumber { get; set; }

        public List<ObjectReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<ObjectReport>(
                        "CALL sp_get_object_report({0})",
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

        public class ObjectReport
        {
            public int WorkNumber { get; set; }
            public string WorkTypeName { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
            public DateTime? RealStartDate { get; set; }
            public DateTime? RealEndDate { get; set; }
            public string? Materials { get; set; }
            public decimal? RealCost { get; set; }
            public decimal? CostDifference { get; set; }
            public int? StartDateDifference { get; set; }
            public int? EndDateDifference { get; set; }
        }
    }
}