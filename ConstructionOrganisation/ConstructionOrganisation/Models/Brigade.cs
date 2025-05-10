using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Brigade
{
    public int BrigadeId { get; set; }

    public int? Foreman { get; set; }

    public string BrigadeName { get; set; } = null!;

    public virtual ICollection<BrigadeEmployee> BrigadeEmployees { get; set; } = new List<BrigadeEmployee>();

    public virtual Employee? ForemanNavigation { get; set; }

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
