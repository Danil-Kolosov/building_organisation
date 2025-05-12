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

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    return RedirectToAction("Login");
        //}

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Login", "Account");
        }
    }
}