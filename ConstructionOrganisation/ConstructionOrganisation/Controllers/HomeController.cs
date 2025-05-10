using ConstructionOrganisation.Atributes;
using ConstructionOrganisation.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionOrganisation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly MySqlAuthService _authService;

        public HomeController(MySqlAuthService authService)
        {
            _authService = authService;
        }

        [AuthorizeRole("admin")]
        public async Task<IActionResult> Admin()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("Password");

            var products = await _authService.GetProducts(username, password);
            return View(products);
        }

        [AuthorizeRole("minadmin")]
        public async Task<IActionResult> MinAdmin()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("Password");

            var products = await _authService.GetProducts(username, password);
            return View(products);
        }

        [AuthorizeRole("readonly")]
        public async Task<IActionResult> ReadOnly()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("Password");

            var products = await _authService.GetProducts(username, password);
            return View(products);
        }
    }
}