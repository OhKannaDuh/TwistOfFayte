using System;
using System.Collections.Generic;
using Lumina.Excel.Sheets;
using Ocelot.Config;
using Ocelot.Config.Fields;
using TwistOfFayte.Config.Excel;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("general")]
public class CurrencyConfig : IAutoConfig
{
    [ExcelOrderedMultiSelect<TerritoryType, ZoneDisplay, ZoneFilter>]
    public List<uint> Zones { get; set; } = [];

    // [Checkbox] public bool ShouldHopInstances { get; set; }= false;

    [IntRange(5, 300)] public int WaitForFateDelay { get; set; } = 30;
}
