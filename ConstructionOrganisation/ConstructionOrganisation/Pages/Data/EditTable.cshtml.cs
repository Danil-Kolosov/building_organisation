using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Data;
using System.Text;
using System.Text.Json;

namespace ConstructionOrganisation.Pages.Data
{
    [IgnoreAntiforgeryToken]
    public class EditTableModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string TableName { get; set; }

        public DataTable TableData { get; set; } = new DataTable();
        public List<string> PrimaryKeys { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }
        public bool CanSelect { get; private set; }
        public bool CanInsert { get; private set; }
        public bool CanUpdate { get; private set; }
        public bool CanDelete { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToPage("/Account/Login");

            if (string.IsNullOrEmpty(TableName))
            {
                ErrorMessage = "Имя таблицы не указано";
                return Page();
            }

            try
            {
                await CheckPermissions();
                await LoadTableStructure();

                if (CanSelect)
                {
                    await LoadTableData();
                }
                else
                {
                    ErrorMessage = "Ошибка: Нет прав на просмотр этой таблицы";
                }

                return Page();
            }
            catch (MySqlException ex)
            {
                ErrorMessage = $"Ошибка MySQL: {ex.Message}";
                return Page();
            }
        }

        public class SaveChangesRequest
        {
            public string TableName { get; set; }
            public JsonElement Changes { get; set; }
            public List<string> PrimaryKeys { get; set; }
        }

        public async Task<IActionResult> OnPostSaveAsync([FromBody] SaveChangesRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TableName))
                {
                    throw new ArgumentException("Имя таблицы не может быть пустым");
                }

                var changes = new
                {
                    updates = request.Changes.GetProperty("updates").EnumerateArray().Select(e => new
                    {
                        key = e.GetProperty("key").EnumerateArray().Select(k => k.ToString()).ToList(),
                        values = e.GetProperty("values").EnumerateObject().ToDictionary(
                            v => v.Name,
                            v => v.Value.ValueKind == JsonValueKind.Null ? null : v.Value.ToString())
                    }).ToList(),
                    inserts = request.Changes.GetProperty("inserts").EnumerateArray().Select(e =>
                        e.EnumerateObject().ToDictionary(
                            v => v.Name,
                            v => v.Value.ValueKind == JsonValueKind.Null ? null : v.Value.ToString())
                    ).ToList(),
                    deletes = request.Changes.GetProperty("deletes").EnumerateArray().Select(e => new
                    {
                        key = e.GetProperty("key").EnumerateArray().Select(k => k.ToString()).ToList()
                    }).ToList()
                };

                using var connection = new MySqlConnection(GetConnectionString());
                await connection.OpenAsync();

                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    // Обработка вставок
                    foreach (var insert in changes.inserts)
                    {
                        var (sql, parameters) = BuildInsertQuery(request.TableName, insert);
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    // Обработка обновлений
                    foreach (var update in changes.updates)
                    {
                        var (sql, parameters) = BuildUpdateQuery(request.TableName, update, request.PrimaryKeys);
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    // Обработка удалений
                    foreach (var delete in changes.deletes)
                    {
                        var (sql, parameters) = BuildDeleteQuery(request.TableName, delete, request.PrimaryKeys);
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    await transaction.CommitAsync();
                    return new JsonResult(new { success = true, message = "Изменения сохранены" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private async Task ExecuteCommand(MySqlConnection connection, string sql,
            List<MySqlParameter> parameters, MySqlTransaction transaction)
        {
            using var cmd = new MySqlCommand(sql, connection, transaction);
            cmd.Parameters.AddRange(parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }

        private (string sql, List<MySqlParameter> parameters) BuildInsertQuery(
            string tableName, Dictionary<string, string> row)
        {
            var columns = new List<string>();
            var values = new List<string>();
            var parameters = new List<MySqlParameter>();

            foreach (var kv in row)
            {
                columns.Add(string.Format("`{0}`", kv.Key));
                values.Add(string.Format("@val_{0}", kv.Key));
                parameters.Add(new MySqlParameter(
                    string.Format("@val_{0}", kv.Key),
                    kv.Value ?? (object)DBNull.Value));
            }

            var sql = string.Format("INSERT INTO `{0}` ({1}) VALUES ({2})",
                tableName,
                string.Join(", ", columns),
                string.Join(", ", values));

            return (sql, parameters);
        }

        private (string sql, List<MySqlParameter> parameters) BuildUpdateQuery(
            string tableName, dynamic change, List<string> primaryKeys)
        {
            var setParts = new List<string>();
            var whereParts = new List<string>();
            var parameters = new List<MySqlParameter>();

            var values = (Dictionary<string, string>)change.values;
            foreach (var kv in values)
            {
                setParts.Add(string.Format("`{0}` = @set_{0}", kv.Key));
                parameters.Add(new MySqlParameter(
                    string.Format("@set_{0}", kv.Key),
                    kv.Value ?? (object)DBNull.Value));
            }

            var keyValues = (List<string>)change.key;
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                whereParts.Add(string.Format("`{0}` = @key_{1}", primaryKeys[i], i));
                parameters.Add(new MySqlParameter(
                    string.Format("@key_{0}", i),
                    keyValues[i] ?? (object)DBNull.Value));
            }

            var sql = string.Format("UPDATE `{0}` SET {1} WHERE {2}",
                tableName,
                string.Join(", ", setParts),
                string.Join(" AND ", whereParts));

            return (sql, parameters);
        }

        private (string sql, List<MySqlParameter> parameters) BuildDeleteQuery(
            string tableName, dynamic row, List<string> primaryKeys)
        {
            var whereParts = new List<string>();
            var parameters = new List<MySqlParameter>();

            var keyValues = (List<string>)row.key;
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                whereParts.Add(string.Format("`{0}` = @key_{1}", primaryKeys[i], i));
                parameters.Add(new MySqlParameter(
                    string.Format("@key_{0}", i),
                    keyValues[i] ?? (object)DBNull.Value));
            }

            var sql = string.Format("DELETE FROM `{0}` WHERE {1}",
                tableName,
                string.Join(" AND ", whereParts));

            return (sql, parameters);
        }

        private async Task CheckPermissions()
        {
            var currentUser = HttpContext.Session.GetString("Username");
            var rootConnectionString = "Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;";

            CanSelect = await CheckPermissionWithNewConnectionAsync(rootConnectionString, currentUser, "SELECT");

            if (!CanSelect)
            {
                CanInsert = CanUpdate = CanDelete = false;
                return;
            }

            var tasks = new List<Task<bool>>();
            var permissions = new[] { "INSERT", "UPDATE", "DELETE" };

            foreach (var perm in permissions)
            {
                tasks.Add(CheckPermissionWithNewConnectionAsync(rootConnectionString, currentUser, perm));
            }

            await Task.WhenAll(tasks);

            CanInsert = tasks[0].Result;
            CanUpdate = tasks[1].Result;
            CanDelete = tasks[2].Result;
        }

        private async Task<bool> CheckPermissionWithNewConnectionAsync(string connectionString, string username, string permission)
        {
            using var connection = new MySqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                return await CheckSinglePermissionAsync(connection, username, permission);
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckSinglePermissionAsync(MySqlConnection connection, string username, string permission)
        {
            var sql = string.Format(@"
                SELECT COUNT(*) 
                FROM mysql.tables_priv 
                WHERE 
                    Db = 'building_organisation' AND
                    Table_name = '{0}' AND
                    User = '{1}' AND
                    Host IN ('%', 'localhost') AND
                    table_priv LIKE '%{2}%'",
                TableName, username, permission);

            using var cmd = new MySqlCommand(sql, connection);
            if ((long)(await cmd.ExecuteScalarAsync() ?? 0) > 0)
            {
                return true;
            }

            var hostsToCheck = new[] { "localhost", "%" };
            foreach (var host in hostsToCheck)
            {
                try
                {
                    sql = string.Format("SHOW GRANTS FOR '{0}'@'{1}'", username, host);
                    using var grantsCmd = new MySqlCommand(sql, connection);
                    using var reader = await grantsCmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var grant = reader.GetString(0).ToUpper();
                        if (grant.Contains("ALL PRIVILEGES") ||
                            grant.Contains("GRANT OPTION") ||
                            grant.Contains(string.Format("{0} ON", permission.ToUpper())))
                        {
                            return true;
                        }
                    }
                }
                catch (MySqlException ex) when (ex.Number == 1141)
                {
                    continue;
                }
            }

            return false;
        }

        private async Task LoadTableStructure()
        {
            using var connection = new MySqlConnection(GetConnectionString());
            await connection.OpenAsync();

            using var cmdKeys = new MySqlCommand(
                string.Format("SHOW KEYS FROM `{0}` WHERE Key_name = 'PRIMARY'", TableName),
                connection);

            using var reader = await cmdKeys.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                PrimaryKeys.Add(reader.GetString("Column_name"));
            }
        }

        private async Task LoadTableData()
        {
            using var connection = new MySqlConnection(GetConnectionString());
            await connection.OpenAsync();

            using var cmd = new MySqlCommand(
                string.Format("SELECT * FROM `{0}` LIMIT 100", TableName.Replace("`", "``")),
                connection);

            TableData = new DataTable();
            new MySqlDataAdapter(cmd).Fill(TableData);
        }

        private string GetConnectionString()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("DbPassword");
            return string.Format("Server=localhost;Database=building_organisation;User={0};Password={1};",
                username, password);
        }
    }
}













/*
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySqlConnector;
namespace ConstructionOrganisation.Pages.Data;
public class EditTableModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string TableName { get; set; }

    [BindProperty]
    public string SqlQuery { get; set; }

    public DataTable TableData { get; set; }
    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            return RedirectToPage("/Account/Login");

        await LoadTableData();
        return Page();
    }

    //public async Task<IActionResult> OnPostAsync()
    //{
    //    try
    //    {
    //        await ExecuteQuery();
    //        await LoadTableData();
    //    }
    //    catch (MySqlException ex)
    //    {
    //        ErrorMessage = ex.Number == 1142
    //            ? "Ошибка: Нет прав на это действие"
    //            : $"Ошибка: {ex.Message}";
    //    }

    //    return Page();
    //}

    public async Task<IActionResult> OnPostSaveAsync(string tableName, List<Dictionary<string, object>> changes)
    {
        try
        {
            var connectionString = GetConnectionString();
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var change in changes)
            {
                var setParts = new List<string>();
                var parameters = new MySqlParameter[change.Count - 1];
                var i = 0;

                foreach (var kv in change)
                {
                    if (kv.Key != "id")
                    {
                        setParts.Add($"`{Model.TableData.Columns[int.Parse(kv.Key)].ColumnName}` = @val{i}");
                        parameters[i] = new MySqlParameter($"@val{i}", kv.Value);
                        i++;
                    }
                }

                var sql = $"UPDATE `{tableName}` SET {string.Join(", ", setParts)} WHERE id = @id";
                using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.Add(new MySqlParameter("@id", change["id"]));
                cmd.Parameters.AddRange(parameters);

                await cmd.ExecuteNonQueryAsync();
            }

            return new OkResult();
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task LoadTableData()
    {
        var connectionString = GetConnectionString();
        using var connection = new MySqlConnection(connectionString);

        await connection.OpenAsync();
        using var cmd = new MySqlCommand($"SELECT * FROM `{TableName}` LIMIT 100", connection);

        TableData = new DataTable();
        new MySqlDataAdapter(cmd).Fill(TableData);
    }

    private async Task ExecuteQuery()
    {
        var connectionString = GetConnectionString();
        using var connection = new MySqlConnection(connectionString);

        await connection.OpenAsync();
        using var cmd = new MySqlCommand(SqlQuery, connection);
        await cmd.ExecuteNonQueryAsync();
    }

    private string GetConnectionString()
    {
        var username = HttpContext.Session.GetString("Username");
        var password = HttpContext.Session.GetString("DbPassword");
        return $"Server=localhost;Database=building_organisation;User={username};Password={password};";
    }
}*/