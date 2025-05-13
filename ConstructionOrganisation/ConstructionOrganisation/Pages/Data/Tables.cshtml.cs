using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Data;

namespace ConstructionOrganisation.Pages.Data
{
    public class TablesModel : PageModel
    {
        public List<string> AvailableTables { get; set; } = new();
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToPage("/Account/Login");

            try
            {
                await LoadAvailableTables();
                return Page();
            }
            catch (MySqlException ex)
            {
                ErrorMessage = $"Ошибка при получении списка таблиц: {ex.Message}";
                return Page();
            }
        }

        private async Task LoadAvailableTables()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("DbPassword");

            using var connection = new MySqlConnection(
                $"Server=localhost;Database=information_schema;User={username};Password={password};");

            await connection.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT TABLE_NAME FROM TABLES WHERE TABLE_SCHEMA = 'building_organisation'",
                connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                AvailableTables.Add(reader.GetString(0));
            }
        }
    }
}








/*
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionOrganisation.Pages.Data;
public class TablesModel : PageModel
{
    public List<string> AvailableTables { get; } = new()
    {
        "employee",
        "projects",
        "contracts"
    };

    public string ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            return RedirectToPage("/Account/Login");

        return Page();
    }
}*/