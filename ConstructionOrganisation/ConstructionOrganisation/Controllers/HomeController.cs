using Microsoft.AspNetCore.Mvc;

namespace ConstructionOrganisation.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Auth");
            return View();
        }
    }
}