using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using ConstructionOrganisation.Models;
using Microsoft.EntityFrameworkCore;

public class QueriesModel : PageModel
{
    private readonly ConstructionOrganisation.Data.ApplicationDbContext _db;

    public QueriesModel(ConstructionOrganisation.Data.ApplicationDbContext db) => _db = db;

    [BindProperty]
    public int QueryId { get; set; }

    public string Result { get; set; }

    public async Task OnPostCallProcedureAsync()
    {
        switch (QueryId)
        {
            case 1:
                var units = await _db.Managements
                    .FromSqlRaw("CALL GetUnitsAndManagers()")
                    .ToListAsync();
                Result = GenerateTable(units);
                break;
        }
    }

    private string GenerateTable(List<Management> data)
    {
        var html = new StringBuilder();
        html.Append("<table class='table'><thead><tr><th>ID</th><th>Название</th></tr></thead><tbody>");

        foreach (var item in data)
            html.Append($"<tr><td>{item.ManagementNumber}</td><td>{item.Director}</td></tr>");

        html.Append("</tbody></table>");
        return html.ToString();
    }
}