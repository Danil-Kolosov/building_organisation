using ConstructionOrganisation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConstructionOrganisation.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MySqlAuthService _authService;

        public LoginModel(MySqlAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [FromQuery] // Получаем параметр из URL
        public bool Logout { get; set; }

        public string ErrorMessage { get; set; }


        public IActionResult OnGet()
        {
            // Если перешли по ссылке выхода
            if (Logout)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete(".AspNetCore.Session");
                return RedirectToPage("/Index");
            }
            // Если уже авторизованы - редирект на главную
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                // Дополнительная проверка на случай прямого POST-запроса
                HttpContext.Session.Clear();
                return RedirectToPage("/Index");
            }
            if (await _authService.ValidateUser(Username, Password))
            {
                HttpContext.Session.SetString("Username", Username);
                HttpContext.Session.SetString("DbPassword", Password); // Сохраняем пароль

                return RedirectToPage("/Reports/Index"); // Перенаправляем на страницу отчетов
            }
            ErrorMessage = "Неверный логин или пароль";
            return Page();

            //if (await _authService.ValidateUser(Username, Password))
            //{
            //    HttpContext.Session.SetString("Username", Username);
            //    return RedirectToPage("/Index");
            //}

            //ErrorMessage = "Неверный логин или пароль";
            //return Page();
        }
    }
}





/*using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ConstructionOrganisation.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Логин")]
            public string UserName { get; set; } // Было Email

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Input.UserName); // Ищем по логину
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded) return LocalRedirect(returnUrl);

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return Page();
        }

        public async Task OnGetAsync() => await Task.CompletedTask;

        //старое - с почтой
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _signInManager.UserManager.FindByNameAsync(Input.UserName);
        //        if (user != null)
        //        {
        //            var result = await _signInManager.PasswordSignInAsync(
        //                user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        //            if (result.Succeeded) return RedirectToPage("/Index");
        //        }
        //        ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
        //    }
        //    return Page();
        //}
    }
}*/