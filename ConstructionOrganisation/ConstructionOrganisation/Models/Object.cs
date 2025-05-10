using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class Object
{
    public int ObjectNameId { get; set; }

    public int? Supervisor { get; set; }

    public int? SectionNameId { get; set; }

    public int? ContractNumber { get; set; }

    public int? ObjectTypeId { get; set; }

    public string ObjectName { get; set; } = null!;

    public virtual Contract? ContractNumberNavigation { get; set; }

    public virtual ICollection<ObjectCharacteristic> ObjectCharacteristics { get; set; } = new List<ObjectCharacteristic>();

    public virtual ObjectType? ObjectType { get; set; }

    public virtual Section? SectionName { get; set; }

    public virtual Employee? SupervisorNavigation { get; set; }

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
