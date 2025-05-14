using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionOrganisation.Pages.Reports
{
    public class sp_get_object_brigade_members : PageModel
    {
        private readonly ApplicationDbContext _context;

        public sp_get_object_brigade_members(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ObjectNumber { get; set; }

        public List<BrigadeMemberReport> ReportData { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                ReportData = await _context.Database
                    .SqlQueryRaw<BrigadeMemberReport>(
                        "CALL sp_get_object_brigade_members({0})",
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

        public class BrigadeMemberReport
        {
            public int BrigadeID { get; set; }
            public string BrigadeName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string GroupName { get; set; }
        }
    }
}