using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Engineering(int managementId = 1)
        {
            try
            {
                var result = await _context.Database
                    .SqlQueryRaw<EngineeringEmployeeReport>(
                        "CALL sp_get_engineering_employee_management({0})",
                        managementId)
                    .AsNoTracking()
                    .ToListAsync();

                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении отчета: {ex.Message}");
                return View("Error");
            }
        }
    }

    public class EngineeringEmployeeReport
    {
        public int EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime HireDate { get; set; } // Изменили на DateTime
        public string Position { get; set; }
    }
}