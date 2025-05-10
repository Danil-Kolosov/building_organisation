using ConstructionOrganisation.Models;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using System.Data;
namespace ConstructionOrganisation.Service{
    public class MySqlAuthService
    {
        private readonly string _connectionString;

        public MySqlAuthService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<User> Authenticate(string username, string password)
        {
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlConnector.MySqlCommand(
                "SELECT id, username, password, role FROM users WHERE username = @username",
                connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // В реальном приложении используйте хеширование паролей!
                if (password == reader.GetString("password"))
                {
                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        Role = reader.GetString("role")
                    };
                }
            }

            return null;
        }

        public async Task<DataTable> GetProducts(string username, string password)
        {
            // Подключение с учетными данными пользователя
            var userConnectionString = $"Server=your-mysql-server;Database=my_app_db;User={username};Password={password};";

            using var connection = new MySqlConnector.MySqlConnection(userConnectionString);
            await connection.OpenAsync();

            var command = new MySqlConnector.MySqlCommand("SELECT * FROM products", connection);
            using var adapter = new MySqlConnector.MySqlDataAdapter(command);

            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }
    }
}