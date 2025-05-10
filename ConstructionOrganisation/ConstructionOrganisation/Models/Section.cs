using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Section
{
    public int SectionNameId { get; set; }

    public int Manager { get; set; }

    public int? ManagementNumber { get; set; }

    public string SectionName { get; set; } = null!;

    public virtual Employee ManagerNavigation { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
