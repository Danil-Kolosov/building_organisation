using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Group
{
    public int GroupNameId { get; set; }

    public string GroupName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<GroupCharacteristic> GroupCharacteristics { get; set; } = new List<GroupCharacteristic>();
}
