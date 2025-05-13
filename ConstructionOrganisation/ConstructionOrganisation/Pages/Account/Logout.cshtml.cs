using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConstructionOrganisation.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToPage("/Index");
        }
    }
}








/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace ConstructionOrganisation.Pages.Account
{
    public class LogoutModel : PageModel
    {

        public IActionResult OnPost()
        {
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("DbPassword");
            return RedirectToPage("/Index");
        }

        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
            // Если нужен синхронный GET-выход (не рекомендуется)
            ClearSession();
            return RedirectToPage("/Account/Login");
        }


        //public async Task<IActionResult> OnPostAsync()
        //{
        //    await HttpContext.Session.LoadAsync();
        //    HttpContext.Session.Clear();
        //    await HttpContext.Session.CommitAsync();
        //    Response.Cookies.Delete(".AspNetCore.Session");
        //    HttpContext.Session.SetString("Username", "ОПАААААААААА");
        //    return RedirectToPage("/Account/Login");
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
            Console.WriteLine("Метод выхода вызван!"); // Проверка в консоли сервера
            await HttpContext.Session.LoadAsync();
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();
            Response.Cookies.Delete(".AspNetCore.Session");
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
}*/