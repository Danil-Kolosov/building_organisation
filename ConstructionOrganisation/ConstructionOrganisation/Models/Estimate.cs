using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Estimate
{
    public int WorkNumber { get; set; }

    public int MaterialId { get; set; }

    public decimal? Cost { get; set; }

    public decimal PlannedQuantity { get; set; }

    public decimal? RealQuantity { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Work WorkNumberNavigation { get; set; } = null!;
}
