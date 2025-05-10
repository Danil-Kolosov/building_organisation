using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class BrigadeEmployee
{
    public int EmployeeCode { get; set; }

    public int BrigadeId { get; set; }

    public virtual Brigade Brigade { get; set; } = null!;

    public virtual Employee EmployeeCodeNavigation { get; set; } = null!;
}
