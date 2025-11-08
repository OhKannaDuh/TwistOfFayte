using System;
using System.Collections.Generic;
using Lumina.Excel.Sheets;
using Ocelot.Config;
using Ocelot.Config.Fields;
using Ocelot.Services.ClientState;
using TwistOfFayte.Config.Excel;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_selection")]
public class MultiZoneConfig : IAutoConfig
{
    [ExcelOrderedMultiSelect<TerritoryType, ZoneDisplay, ZoneFilter>]
    public List<uint> Zones { get; set; } = [];

    // [Checkbox] public bool ShouldHopInstances { get; set; }= false;

    [IntRange(5, 300)] public int WaitForFateDelay { get; set; } = 30;

    public uint GetNextZone(IClient client)
    {
        if (Zones.Count == 0)
        {
            return 0;
        }

        var current = (uint)client.CurrentTerritoryId;

        var index = Zones.IndexOf(current);

        if (index == -1)
        {
            return Zones[0];
        }

        var nextIndex = (index + 1) % Zones.Count;
        return Zones[nextIndex];
    }
}
