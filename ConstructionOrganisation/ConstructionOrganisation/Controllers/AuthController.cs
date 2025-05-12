using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionOrganisation.Controllers
{
    public class AuthController : Controller
    {
        private readonly MySqlAuthService _authService;

        public AuthController(MySqlAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (await _authService.ValidateUser(username, password))
            {
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}