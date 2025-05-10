using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Management
{
    public int ManagementNumber { get; set; }

    public int Director { get; set; }

    public virtual Employee DirectorNavigation { get; set; } = null!;

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
