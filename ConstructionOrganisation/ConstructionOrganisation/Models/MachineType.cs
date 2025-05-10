using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class MachineType
{
    public int MachineTypeId { get; set; }

    public string MachineType1 { get; set; } = null!;

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
