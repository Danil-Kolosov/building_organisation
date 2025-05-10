using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Employee
{
    public int EmployeeCode { get; set; }

    public int? GroupNameId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public DateOnly? HireDate { get; set; }

    public virtual BrigadeEmployee? BrigadeEmployee { get; set; }

    public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();

    public virtual Group? GroupName { get; set; }

    public virtual Management? Management { get; set; }

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
}
