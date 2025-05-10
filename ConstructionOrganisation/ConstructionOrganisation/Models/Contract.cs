using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Contract
{
    public int ContractNumber { get; set; }

    public decimal Price { get; set; }

    public string CustomerName { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();
}
