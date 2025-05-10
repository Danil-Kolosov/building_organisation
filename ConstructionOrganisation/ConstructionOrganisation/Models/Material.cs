using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string? MeasurementUnits { get; set; }

    public string MaterialName { get; set; } = null!;

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();
}
