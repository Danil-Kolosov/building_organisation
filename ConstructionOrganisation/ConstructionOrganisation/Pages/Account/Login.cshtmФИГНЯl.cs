//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
//using System;

//namespace ConstructionOrganisation.Pages.Account
//{
//    public class LoginModel : PageModel
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly UserManager<IdentityUser> _userManager;

//        public LoginModel(SignInManager<IdentityUser> signInManager,
//                         UserManager<IdentityUser> userManager)
//        {
//            _signInManager = signInManager;
//            _userManager = userManager;
//        }

//        [BindProperty]
//        public InputModel Input { get; set; }

//        public string ReturnUrl { get; set; }

//        public class InputModel
//        {
//            [Required]
//            [Display(Name = "Логин")]
//            public string UserName { get; set; } // Используем UserName вместо Email

//            [Required]
//            [DataType(DataType.Password)]
//            public string Password { get; set; }

//            [Display(Name = "Запомнить меня")]
//            public bool RememberMe { get; set; }
//        }

//        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        {
//            returnUrl ??= Url.Content("~/");

//            if (ModelState.IsValid)
//            {
//                // Ищем пользователя по логину
//                var user = await _userManager.FindByNameAsync(Input.UserName);

//                if (user != null)
//                {
//                    var result = await _signInManager.PasswordSignInAsync(
//                        user.UserName, // Используем UserName для входа
//                        Input.Password,
//                        Input.RememberMe,
//                        lockoutOnFailure: false);

//                    if (result.Succeeded)
//                        return LocalRedirect(returnUrl);
//                }

//                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
//            }

//            return Page();
//        }
//    }

//}





///*using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.ComponentModel.DataAnnotations;

//namespace ConstructionOrganisation.Pages.Account
//{
//    public class LoginModel : PageModel
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly ILogger<LoginModel> _logger;

//        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
//        {
//            _signInManager = signInManager;
//            _logger = logger;
//        }

//        [BindProperty]
//        public InputModel Input { get; set; }

//        public class InputModel
//        {
//            [Required]
//            [Display(Name = "Логин")]
//            public string UserName { get; set; } // Было Email

//            [Required]
//            [DataType(DataType.Password)]
//            [Display(Name = "Пароль")]
//            public string Password { get; set; }

//            [Display(Name = "Запомнить меня")]
//            public bool RememberMe { get; set; }
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            var user = await _userManager.FindByNameAsync(Input.UserName); // Ищем по логину
//            if (user == null)
//            {
//                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
//                return Page();
//            }

//            var result = await _signInManager.PasswordSignInAsync(
//                user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

//            if (result.Succeeded) return LocalRedirect(returnUrl);

//            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
//            return Page();
//        }

//        public async Task OnGetAsync() => await Task.CompletedTask;

//        //старое - с почтой
//        //public async Task<IActionResult> OnPostAsync()
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        var user = await _signInManager.UserManager.FindByNameAsync(Input.UserName);
//        //        if (user != null)
//        //        {
//        //            var result = await _signInManager.PasswordSignInAsync(
//        //                user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

//        //            if (result.Succeeded) return RedirectToPage("/Index");
//        //        }
//        //        ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
//        //    }
//        //    return Page();
//        //}
//    }
//}*/