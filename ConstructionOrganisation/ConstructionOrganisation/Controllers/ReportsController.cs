using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("construction-units")]
        public async Task<IActionResult> GetConstructionUnits()
        {
            var units = await _context.Managements
                .Include(u => u.Director)
                .ToListAsync();

            return Ok(units);
        }
    }
}