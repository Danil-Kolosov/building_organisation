using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace ConstructionOrganisation.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Если нужен синхронный GET-выход (не рекомендуется)
            ClearSession();
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("OnPostAsync вызван!"); // Проверьте вывод в консоли сервера
            await HttpContext.Session.LoadAsync();
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();

            Response.Cookies.Delete(".AspNetCore.Session");
            HttpContext.Session.SetString("Username", "ОПАААААААААА");
            return RedirectToPage("/Account/Login");
        }

        private void ClearSession()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true
            });
        }

        private async Task ClearSessionAsync()
        {
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync(); // Явное сохранение изменений

            Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true
            });
        }
    }
}