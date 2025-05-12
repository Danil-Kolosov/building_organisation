using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConstructionOrganisation.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {

            Console.WriteLine("GET-запрос на выход получен!");
            HttpContext.Session.Clear();
            return RedirectToPage("/Account/Login");
            //HttpContext.Session.Clear();
            //return RedirectToPage("/Account/Login"); // Редирект на страницу входа
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.Keys.Contains("Username"))
            {
                Console.WriteLine($"Выход: {HttpContext.Session.GetString("Username")}");
                HttpContext.Session.Remove("Username");
            }

            // Явное удаление куки
            Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true
            });

            return RedirectToPage("/Account/Login");
            //HttpContext.Session.Clear();
            //return RedirectToPage("/Account/Login");
        }
    }
}