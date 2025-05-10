using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class SectionEmployee
{
    public int SectionNameId { get; set; }

    public int EmployeeCode { get; set; }

    public virtual Employee EmployeeCodeNavigation { get; set; } = null!;
}
