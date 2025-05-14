using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_objects_with_works_on_date_in_management : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_objects_with_works_on_date_in_management(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string WorkTypeName { get; set; } = "Монтажные";

        [BindProperty(SupportsGet = true)]
        public DateTime FirstDate { get; set; } = DateTime.Today.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime SecondDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public string ManagementNumber { get; set; } = "1";

        public List<ObjectWithWorksManagementReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<ObjectWithWorksManagementReport>(
                        "CALL sp_get_objects_with_works_on_date_in_management({0}, {1}, {2}, {3})",
                        WorkTypeName,
                        FirstDate.ToString("yyyy-MM-dd"),
                        SecondDate.ToString("yyyy-MM-dd"),
                        ManagementNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Report Error: {ex.Message}");
                // Можно добавить TempData для отображения ошибки на странице
            }
        }

        public class ObjectWithWorksManagementReport
        {
            public int ObjectNameID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string SectionName { get; set; }
            public string ObjectType { get; set; }
            public string ObjectName { get; set; }
            public int ContractNumber { get; set; }
            public decimal Price { get; set; }
            public string CustomerName { get; set; }
            public string WorkTypeName { get; set; }
            public DateTime PlannedStartDate { get; set; }
            public DateTime PlannedEndDate { get; set; }
        }
    }
}