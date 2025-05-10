using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Work
{
    public int WorkNumber { get; set; }

    public int ObjectNameId { get; set; }

    public int SectionNameId { get; set; }

    public int WorkTypeId { get; set; }

    public int? BrigadeId { get; set; }

    public DateOnly PlannedStartDate { get; set; }

    public DateOnly PlannedEndDate { get; set; }

    public DateOnly? RealStartDate { get; set; }

    public DateOnly? RealEndDate { get; set; }

    public virtual Brigade? Brigade { get; set; }

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();

    public virtual Object ObjectName { get; set; } = null!;

    public virtual Section SectionName { get; set; } = null!;

    public virtual WorkType WorkType { get; set; } = null!;

    public virtual ICollection<Machine> SerialNumbers { get; set; } = new List<Machine>();
}
