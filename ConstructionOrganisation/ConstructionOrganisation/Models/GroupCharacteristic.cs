using System;
using System.Collections.Generic;

namespace ConstructionOrganisation.Models;

public partial class GroupCharacteristic
{
    public int GroupNameId { get; set; }

    public int CharacteristicGrId { get; set; }

    public string ValueGrCh { get; set; } = null!;

    public virtual CharacteristicGr CharacteristicGr { get; set; } = null!;

    public virtual Group GroupName { get; set; } = null!;
}
