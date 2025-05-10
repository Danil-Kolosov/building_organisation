using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class CharacteristicOb
{
    public int CharacteristicObNameId { get; set; }

    public string CharacteristicObName { get; set; } = null!;

    public virtual ICollection<ObjectCharacteristic> ObjectCharacteristics { get; set; } = new List<ObjectCharacteristic>();
}
