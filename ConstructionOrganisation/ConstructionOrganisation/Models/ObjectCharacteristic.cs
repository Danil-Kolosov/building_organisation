using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class ObjectCharacteristic
{
    public int ObjectNameId { get; set; }

    public int CharacteristicObName { get; set; }

    public string ValueObCh { get; set; } = null!;

    public virtual CharacteristicOb CharacteristicObNameNavigation { get; set; } = null!;

    public virtual Object ObjectName { get; set; } = null!;
}
