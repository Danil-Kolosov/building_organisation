using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_work_type_by_brigade_on_date_and_objects : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_work_type_by_brigade_on_date_and_objects(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Укажите название бригады")]
        public string BrigadeName { get; set; }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Укажите дату начала")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Укажите дату окончания")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<BrigadeWorkReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (!ModelState.IsValid)
                return;

            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<BrigadeWorkReport>(
                        "CALL sp_work_type_by_brigade_on_date_and_objects({0}, {1}, {2})",
                        BrigadeName,
                        StartDate.ToString("yyyy-MM-dd"),
                        EndDate.ToString("yyyy-MM-dd"))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class BrigadeWorkReport
        {
            public string BrigadeName { get; set; }
            public string WorkTypeName { get; set; }
            public string ObjectName { get; set; }
            public int ObjectNameID { get; set; }
            public int WorkNumber { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
            public DateTime? RealStartDate { get; set; }
            public DateTime? RealEndDate { get; set; }
            public int? RealDurationDays { get; set; }
            public string Delay { get; set; }
        }
    }
}