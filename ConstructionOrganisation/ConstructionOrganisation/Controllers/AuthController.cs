using ConstructionOrganisation.Service;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _authService.Authenticate(username, password);

            if (user == null)
            {
                ViewBag.Error = "Неверное имя пользователя или пароль";
                return View();
            }

            // Сохраняем пользователя в сессии
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Password", password); // В реальном приложении так делать не стоит!

            // Перенаправляем по роли
            return user.Role switch
            {
                "admin" => RedirectToAction("Admin", "Home"),
                "minadmin" => RedirectToAction("MinAdmin", "Home"),
                _ => RedirectToAction("ReadOnly", "Home")
            };
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}