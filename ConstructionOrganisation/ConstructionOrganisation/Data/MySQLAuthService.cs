using MySqlConnector;

namespace ConstructionOrganisation.Data
{
    public class MySqlAuthService
    {
        private readonly string _adminConnectionString;

        public MySqlAuthService(IConfiguration config)
        {
            _adminConnectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> ValidateUser(string username, string password)
        {
            try
            {
                // Проверяем учетные данные через прямое подключение
                var userConnectionString = $"Server=localhost;Database=building_organisation;User={username};Password={password};";
                using var connection = new MySqlConnection(userConnectionString);
                await connection.OpenAsync();

                // Проверяем, есть ли пользователь в системной таблице mysql.user
                using var checkCmd = new MySqlConnection(_adminConnectionString);
                await checkCmd.OpenAsync();

                using var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM mysql.user WHERE User = @username AND authentication_string IS NOT NULL",
                    checkCmd);

                cmd.Parameters.AddWithValue("@username", username);
                var count = Convert.ToInt64(await cmd.ExecuteScalarAsync());

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка авторизации: {ex.Message}");
                return false;
            }
        }
    }
}

//public async Task<bool> ValidateUser(string username, string password)
//{
//    try
//    {
//        Console.WriteLine($"Попытка входа: {username}");
//        var connectionString = $"Server=localhost;Database=mysql;User={username};Password={password};";
//        using var connection = new MySqlConnection(connectionString);
//        await connection.OpenAsync();
//        Console.WriteLine("Успешное подключение к MySQL");
//        return true;
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Ошибка авторизации: {ex.Message}");
//        return false;
//    }
//}
//    }
//}