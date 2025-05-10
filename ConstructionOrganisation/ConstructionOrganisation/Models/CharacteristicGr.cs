using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class CharacteristicGr
{
    public int CharacteristicGrId { get; set; }

    public string CharacteristicGrName { get; set; } = null!;

    public virtual ICollection<GroupCharacteristic> GroupCharacteristics { get; set; } = new List<GroupCharacteristic>();
}
