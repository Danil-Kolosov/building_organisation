using MySqlConnector;

namespace ConstructionOrganisation.Services
{
    public class AdminCheckerService
    {
        public async Task<bool> IsAdminAsync(string username, string password)
        {
            try
            {
                var connectionString = $"Server=localhost;Database=mysql;User={username};Password={password};";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                using var cmd = new MySqlCommand(
                    "SELECT Super_priv FROM mysql.user WHERE User = CURRENT_USER() LIMIT 1",
                    connection);

                return (await cmd.ExecuteScalarAsync())?.ToString() == "Y";
            }
            catch
            {
                return false;
            }
        }
    }
}