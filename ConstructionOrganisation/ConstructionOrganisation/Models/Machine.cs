using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Machine
{
    public int SerialNumber { get; set; }

    public int? ManagementNumber { get; set; }

    public int? MachineTypeId { get; set; }

    public virtual MachineType? MachineType { get; set; }

    public virtual Management? ManagementNumberNavigation { get; set; }

    public virtual ICollection<Work> WorkNumbers { get; set; } = new List<Work>();
}
