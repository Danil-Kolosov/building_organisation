using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class ObjectType
{
    public int ObjectTypeId { get; set; }

    public string ObjectType1 { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();
}
