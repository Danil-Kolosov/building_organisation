using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Data;
using System.Text;
using System.Text.Json;

namespace ConstructionOrganisation.Pages.Data
{
    public class EditTableModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string TableName { get; set; }

        public DataTable TableData { get; set; } = new DataTable(); // Инициализация по умолчанию
        public List<string> PrimaryKeys { get; set; } = new List<string>(); // Инициализация по умолчани
        public string ErrorMessage { get; set; }
        public bool CanSelect { get; private set; }
        public bool CanInsert { get; private set; }
        public bool CanUpdate { get; private set; }
        public bool CanDelete { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToPage("/Account/Login");

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
        // Добавьте этот класс в тот же файл, но вне класса EditTableModel
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
                Console.WriteLine($"Получен запрос для таблицы: {request.TableName}");
                Console.WriteLine($"PrimaryKeys: {string.Join(", ", request.PrimaryKeys)}");

                if (string.IsNullOrEmpty(request.TableName))
                {
                    throw new ArgumentException("Имя таблицы не может быть пустым");
                }

                // Полностью переработанная обработка изменений
                var changes = new Dictionary<string, List<Dictionary<string, object>>>
                {
                    ["updates"] = new List<Dictionary<string, object>>(),
                    ["inserts"] = new List<Dictionary<string, object>>(),
                    ["deletes"] = new List<Dictionary<string, object>>()
                };

                // Обработка вставок
                if (request.Changes.TryGetProperty("inserts", out var inserts) && inserts.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in inserts.EnumerateArray())
                    {
                        changes["inserts"].Add(ConvertJsonElementToDict(item));
                    }
                }

                // Обработка обновлений
                if (request.Changes.TryGetProperty("updates", out var updates) && updates.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in updates.EnumerateArray())
                    {
                        var updateDict = new Dictionary<string, object>();

                        if (item.TryGetProperty("key", out var keyElement))
                        {
                            updateDict["key"] = ConvertJsonElementToList(keyElement);
                        }

                        if (item.TryGetProperty("values", out var valuesElement))
                        {
                            updateDict["values"] = ConvertJsonElementToDict(valuesElement);
                        }

                        changes["updates"].Add(updateDict);
                    }
                }

                // Обработка удалений
                if (request.Changes.TryGetProperty("deletes", out var deletes) && deletes.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in deletes.EnumerateArray())
                    {
                        var deleteDict = new Dictionary<string, object>();

                        if (item.TryGetProperty("key", out var keyElement))
                        {
                            deleteDict["key"] = ConvertJsonElementToList(keyElement);
                        }

                        changes["deletes"].Add(deleteDict);
                    }
                }

                Console.WriteLine("Changes processed:");
                Console.WriteLine(JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }));

                // Остальной код метода остается без изменений
                using var connection = new MySqlConnection(GetConnectionString());
                await connection.OpenAsync();

                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    // Обработка вставок
                    foreach (var insert in changes["inserts"])
                    {
                        var (sql, parameters) = BuildInsertQuery(request.TableName, insert);
                        Console.WriteLine($"Executing INSERT: {sql}");
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    // Обработка обновлений
                    foreach (var update in changes["updates"])
                    {
                        var (sql, parameters) = BuildUpdateQuery(request.TableName, update, request.PrimaryKeys);
                        Console.WriteLine($"Executing UPDATE: {sql}");
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    // Обработка удалений
                    foreach (var delete in changes["deletes"])
                    {
                        var (sql, parameters) = BuildDeleteQuery(request.TableName, delete, request.PrimaryKeys);
                        Console.WriteLine($"Executing DELETE: {sql}");
                        await ExecuteCommand(connection, sql, parameters, transaction);
                    }

                    await transaction.CommitAsync();
                    return new JsonResult(new { success = true, message = "Изменения сохранены" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Ошибка при сохранении: {ex}");
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex}");
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

        


        // Вспомогательный метод для конвертации списка JsonElement в Dictionary
        //private List<Dictionary<string, object>> ConvertJsonElementList(JsonElement element)
        //{
        //    var result = new List<Dictionary<string, object>>();

        //    foreach (var item in element.EnumerateArray())
        //    {
        //        var dict = new Dictionary<string, object>();

        //        foreach (var property in item.EnumerateObject())
        //        {
        //            dict[property.Name] = ConvertJsonValue(property.Value);
        //        }

        //        result.Add(dict);
        //    }

        //    return result;
        //}

        private List<object> ConvertJsonElementToList(JsonElement element)
        {
            var list = new List<object>();

            foreach (var item in element.EnumerateArray())
            {
                list.Add(item.ValueKind switch
                {
                    JsonValueKind.String => item.GetString(),
                    JsonValueKind.Number => item.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => item.ToString()
                });
            }

            return list;
        }

        // Вспомогательный метод для конвертации JsonElement в конкретные типы
        //private object ConvertJsonValue(JsonElement element)
        //{
        //    return element.ValueKind switch
        //    {
        //        JsonValueKind.String => element.GetString(),
        //        JsonValueKind.Number => element.GetDecimal(),
        //        JsonValueKind.True => true,
        //        JsonValueKind.False => false,
        //        JsonValueKind.Null => null,
        //        JsonValueKind.Array => ConvertJsonElementList(element),
        //        JsonValueKind.Object => ConvertJsonElementToDict(element),
        //        _ => element.ToString()
        //    };
        //}

        // Вспомогательный метод для конвертации JsonElement в Dictionary
        private Dictionary<string, object> ConvertJsonElementToDict(JsonElement element)
        {
            var dict = new Dictionary<string, object>();

            foreach (var property in element.EnumerateObject())
            {
                dict[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.Number => property.Value.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Array => ConvertJsonElementToList(property.Value),
                    _ => property.Value.ToString()
                };
            }

            return dict;
        }

        private (string sql, List<MySqlParameter> parameters) BuildInsertQuery(
    string tableName, Dictionary<string, object> row)
        {
            var columns = new List<string>();
            var values = new List<string>();
            var parameters = new List<MySqlParameter>();

            foreach (var kv in row)
            {
                columns.Add($"`{kv.Key}`");
                values.Add($"@val_{kv.Key}");
                parameters.Add(new MySqlParameter($"@val_{kv.Key}", kv.Value ?? DBNull.Value));
            }

            var sql = $"INSERT INTO `{tableName}` ({string.Join(", ", columns)}) " +
                      $"VALUES ({string.Join(", ", values)})";

            return (sql, parameters);
        }

        private (string sql, List<MySqlParameter> parameters) BuildUpdateQuery(
    string tableName, Dictionary<string, object> change, List<string> primaryKeys)
        {
            var setParts = new List<string>();
            var whereParts = new List<string>();
            var parameters = new List<MySqlParameter>();

            // Формируем SET часть
            var values = (Dictionary<string, object>)change["values"];
            foreach (var kv in values)
            {
                setParts.Add($"`{kv.Key}` = @set_{kv.Key}");

                // Нормализация дат
                object value = kv.Value;
                if (value is string strValue && DateTime.TryParse(strValue, out var dateValue))
                {
                    value = dateValue.ToString("yyyy-MM-dd");
                }

                parameters.Add(new MySqlParameter($"@set_{kv.Key}", value ?? DBNull.Value));
            }

            // Формируем WHERE часть
            var keyValues = (List<object>)change["key"];
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                whereParts.Add($"`{primaryKeys[i]}` = @key_{i}");
                parameters.Add(new MySqlParameter($"@key_{i}", keyValues[i] ?? DBNull.Value));
            }

            var sql = $"UPDATE `{tableName}` SET {string.Join(", ", setParts)} WHERE {string.Join(" AND ", whereParts)}";
            return (sql, parameters);
        }

        private (string sql, List<MySqlParameter> parameters) BuildDeleteQuery(
    string tableName, Dictionary<string, object> row, List<string> primaryKeys)
        {
            var whereParts = new List<string>();
            var parameters = new List<MySqlParameter>();

            // Формируем WHERE часть из key
            var keyValues = (List<object>)row["key"];
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                whereParts.Add($"`{primaryKeys[i]}` = @key_{i}");
                parameters.Add(new MySqlParameter($"@key_{i}", keyValues[i] ?? DBNull.Value));
            }

            var sql = $"DELETE FROM `{tableName}` WHERE {string.Join(" AND ", whereParts)}";
            return (sql, parameters);
        }

        // Вспомогательный метод для конвертации JsonElement
        private object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => DBNull.Value,
                JsonValueKind.Undefined => DBNull.Value,
                _ => element.ToString()
            };
        }

        //private async Task CheckPermissions()
        //{
        //    using var connection = new MySqlConnection("Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;"/*GetConnectionString()*/);
        //    await connection.OpenAsync();

        //    // Проверяем права для текущего пользователя
        //    var sql = $@"
        //        SELECT 
        //            table_priv 
        //        FROM 
        //            mysql.tables_priv 
        //        WHERE 
        //            Db = 'building_organisation' AND 
        //            Table_name = '{TableName}' AND 
        //            User = '{HttpContext.Session.GetString("Username")}'";

        //    using var cmd = new MySqlCommand(sql, connection);
        //    using var reader = await cmd.ExecuteReaderAsync();

        //    if (reader.HasRows)
        //    {
        //        while (await reader.ReadAsync())
        //        {
        //            var privileges = reader.GetString("table_priv").ToUpper();
        //            CanSelect = privileges.Contains("SELECT");
        //            CanInsert = privileges.Contains("INSERT");
        //            CanUpdate = privileges.Contains("UPDATE");
        //            CanDelete = privileges.Contains("DELETE");
        //            //1 - убрать кнопку сохранить изменения если можно только select + проверка на кнопке чтобы при отсутсвии изменений ошибка не было
        //            //2 - если захожу как root и нажимаю вывести таблицу employee - то ошибка вот такая ""
        //        }
        //    }
        //    else
        //    {
        //        // Если нет конкретных прав на таблицу, проверяем глобальные права
        //        sql = "SHOW GRANTS FOR CURRENT_USER";
        //        using var cmdGlobal = new MySqlCommand(sql, connection);
        //        using var globalReader = await cmdGlobal.ExecuteReaderAsync();

        //        while (await globalReader.ReadAsync())
        //        {
        //            var grant = globalReader.GetString(0).ToUpper();
        //            if (grant.Contains("ALL PRIVILEGES") || grant.Contains("GRANT OPTION"))
        //            {
        //                CanSelect = CanInsert = CanUpdate = CanDelete = true;
        //                break;
        //            }

        //            CanSelect |= grant.Contains("SELECT");
        //            CanInsert |= grant.Contains("INSERT");
        //            CanUpdate |= grant.Contains("UPDATE");
        //            CanDelete |= grant.Contains("DELETE");
        //        }
        //    }
        //}


        //Этот вариант хороший и даже хитрый - но со столбцами при вставке и удалении проблема
        //- права то есть, но столбец надо угадывать - и получается что если столбец
        //несуществующий - то ошибка - а раз ошибка то и прав нет, хотя они то есть
        //private async Task CheckPermissions()
        //{
        //    // Проверка SELECT
        //    try
        //    {
        //        using var selectConn = new MySqlConnection(GetConnectionString());
        //        await selectConn.OpenAsync();
        //        using var selectCmd = new MySqlCommand($"SELECT 1 FROM `{TableName}` LIMIT 1", selectConn);
        //        await selectCmd.ExecuteScalarAsync();
        //        CanSelect = true;
        //    }
        //    catch { CanSelect = false; }

        //    // Проверка INSERT
        //    try
        //    {
        //        using var insertConn = new MySqlConnection(GetConnectionString());
        //        await insertConn.OpenAsync();
        //        using var insertCmd = new MySqlCommand(
        //            $"INSERT INTO `{TableName}` (id) SELECT id FROM `{TableName}` WHERE 1=0",
        //            insertConn);
        //        await insertCmd.ExecuteNonQueryAsync();
        //        CanInsert = true;
        //    }
        //    catch { CanInsert = false; }

        //    // Проверка UPDATE
        //    try
        //    {
        //        using var updateConn = new MySqlConnection(GetConnectionString());
        //        await updateConn.OpenAsync();
        //        using var updateCmd = new MySqlCommand(
        //            $"UPDATE `{TableName}` SET id = id WHERE 1=0",
        //            updateConn);
        //        await updateCmd.ExecuteNonQueryAsync();
        //        CanUpdate = true;
        //    }
        //    catch { CanUpdate = false; }

        //    // Проверка DELETE
        //    try
        //    {
        //        using var deleteConn = new MySqlConnection(GetConnectionString());
        //        await deleteConn.OpenAsync();
        //        using var deleteCmd = new MySqlCommand($"DELETE FROM `{TableName}` WHERE 1=0", deleteConn);
        //        await deleteCmd.ExecuteNonQueryAsync();
        //        CanDelete = true;
        //    }
        //    catch { CanDelete = false; }
        //}        

        //private async Task CheckPermissions()
        //{
        //    // Получаем текущего пользователя из сессии
        //    var currentUser = HttpContext.Session.GetString("Username");

        //    // Используем root-соединение для проверки прав
        //    var rootConnectionString = "Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;";

        //    try
        //    {
        //        using var rootConnection = new MySqlConnection(rootConnectionString);
        //        await rootConnection.OpenAsync();

        //        // Проверяем SELECT первым
        //        CanSelect = await CheckUserPermissionAsync(rootConnection, currentUser, "SELECT");

        //        // Если нет прав на SELECT, остальные проверки бессмысленны
        //        if (!CanSelect)
        //        {
        //            CanInsert = CanUpdate = CanDelete = false;
        //            return;
        //        }

        //        // Проверяем остальные права параллельно для производительности
        //        var insertTask = CheckUserPermissionAsync(rootConnection, currentUser, "INSERT");
        //        var updateTask = CheckUserPermissionAsync(rootConnection, currentUser, "UPDATE");
        //        var deleteTask = CheckUserPermissionAsync(rootConnection, currentUser, "DELETE");

        //        await Task.WhenAll(insertTask, updateTask, deleteTask);

        //        CanInsert = await insertTask;
        //        CanUpdate = await updateTask;
        //        CanDelete = await deleteTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Логируем ошибку, но продолжаем работу
        //        Console.WriteLine($"Ошибка проверки прав: {ex.Message}");
        //        CanSelect = CanInsert = CanUpdate = CanDelete = false;
        //    }
        //}

        private async Task CheckPermissions()
        {
            // Получаем текущего пользователя из сессии
            var currentUser = HttpContext.Session.GetString("Username");

            // Root-соединение будет создаваться отдельно для каждого запроса
            var rootConnectionString = "Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;";

            // 1. Сначала проверяем SELECT (основное право)
            CanSelect = await CheckPermissionWithNewConnectionAsync(rootConnectionString, currentUser, "SELECT");

            // Если нет прав на SELECT, остальные проверки не нужны
            if (!CanSelect)
            {
                CanInsert = CanUpdate = CanDelete = false;
                return;
            }

            // 2. Параллельно проверяем INSERT, UPDATE, DELETE
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
            // Каждый вызов создает и уничтожает собственное соединение
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
            // 1. Проверяем табличные привилегии
            var sql = @$"
        SELECT COUNT(*) 
        FROM mysql.tables_priv 
        WHERE 
            Db = 'building_organisation' AND
            Table_name = '{TableName}' AND
            User = '{username}' AND
            Host IN ('%', 'localhost') AND
            table_priv LIKE '%{permission}%'";

            using var cmd = new MySqlCommand(sql, connection);
            if ((long)(await cmd.ExecuteScalarAsync() ?? 0) > 0)
            {
                return true;
            }

            // 2. Проверяем глобальные привилегии
            var hostsToCheck = new[] { "localhost", "%" };
            foreach (var host in hostsToCheck)
            {
                try
                {
                    sql = $"SHOW GRANTS FOR '{username}'@'{host}'";
                    using var grantsCmd = new MySqlCommand(sql, connection);
                    using var reader = await grantsCmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var grant = reader.GetString(0).ToUpper();
                        if (grant.Contains("ALL PRIVILEGES") ||
                            grant.Contains("GRANT OPTION") ||
                            grant.Contains($"{permission.ToUpper()} ON"))
                        {
                            return true;
                        }
                    }
                }
                catch (MySqlException ex) when (ex.Number == 1141) // ER_NO_SUCH_GRANT
                {
                    continue;
                }
            }

            return false;
        }

        //private async Task<bool> CheckUserPermissionAsync(MySqlConnection rootConnection, string username, string permission)
        //{
        //    try
        //    {
        //        // Проверяем конкретные права на таблицу
        //        var sql = @$"
        //    SELECT COUNT(*) 
        //    FROM mysql.tables_priv 
        //    WHERE 
        //        Db = 'building_organisation' AND
        //        Table_name = '{TableName}' AND
        //        User = '{username}' AND
        //        table_priv LIKE '%{permission}%'";

        //        using var cmd = new MySqlCommand(sql, rootConnection);
        //        var result = (long)(await cmd.ExecuteScalarAsync() ?? 0);

        //        // Если нет конкретных прав, проверяем глобальные
        //        if (result == 0)
        //        {
        //            sql = $"SHOW GRANTS FOR '{username}'@'%'";
        //            using var grantsCmd = new MySqlCommand(sql, rootConnection);
        //            using var reader = await grantsCmd.ExecuteReaderAsync();

        //            while (await reader.ReadAsync())
        //            {
        //                var grant = reader.GetString(0).ToUpper();
        //                if (grant.Contains("ALL PRIVILEGES") ||
        //                    grant.Contains("GRANT OPTION") ||
        //                    grant.Contains($"{permission.ToUpper()} ON"))
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;
        //        }

        //        return result > 0;
        //    }
        //    catch
        //    {
        //        // Если произошла ошибка, считаем что прав нет
        //        return false;
        //    }
        //}

        private async Task LoadTableStructure()
        {
            using var connection = new MySqlConnection(GetConnectionString());
            await connection.OpenAsync();

            // Получаем первичные ключи
            using var cmdKeys = new MySqlCommand(
                $"SHOW KEYS FROM `{TableName}` WHERE Key_name = 'PRIMARY'",
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
                $"SELECT * FROM `{TableName.Replace("`", "``")}` LIMIT 100",
                connection);

            TableData = new DataTable();
            new MySqlDataAdapter(cmd).Fill(TableData);
        }

        private string GetConnectionString()
        {
            var username = HttpContext.Session.GetString("Username");
            var password = HttpContext.Session.GetString("DbPassword");
            return $"Server=localhost;Database=building_organisation;User={username};Password={password};";
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