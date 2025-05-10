using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class WorkType
{
    public int WorkTypeId { get; set; }

    public string WorkTypeName { get; set; } = null!;

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
