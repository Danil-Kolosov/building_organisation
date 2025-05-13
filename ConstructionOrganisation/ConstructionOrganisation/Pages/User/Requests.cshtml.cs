using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Data;

namespace ConstructionOrganisation.Pages.User
{
    public class RequestsModel : PageModel
    {
        [BindProperty]
        public string Query { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public DataTable Results { get; set; }
        public int? AffectedRows { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToPage("/Account/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");
                using (var connection = new MySqlConnection(
                    $"Server=localhost;Database=building_organisation;User={username};Password={Password};"))
                {
                    connection.Open();

                    using (var cmd = new MySqlCommand(Query, connection))
                    {
                        if (Query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            Results = new DataTable();
                            new MySqlDataAdapter(cmd).Fill(Results);
                        }
                        else
                        {
                            AffectedRows = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message} (Код: {ex.Number})";
            }

            return Page();
        }
    }
}