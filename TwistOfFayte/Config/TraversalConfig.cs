using System;
using Lumina.Excel.Sheets;
using Ocelot.Config;
using Ocelot.Config.Fields;
using TwistOfFayte.Config.Excel;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_selection")]
public class TraversalConfig : IAutoConfig
{
    [FloatRange(6f, 20f)] public float CostPerYalm { get; set; } = 20f;

    [IntRange(5, 10)] public int TimeToTeleport { get; set; } = 6;

    [Checkbox] public bool ShouldTeleport { get; set; } = true;

    [ExcelSelect<Mount, MountDisplay, MountFilter>]
    public uint MountId { get; set; } = 1;

    [Checkbox] public bool UseMountRoulette { get; set; } = false;

    // [ExcelOrderedMultiSelect<Mount, MountDisplay, MountFilter>]
    // public List<uint> MountMultiId { get; set; } = [1];
}
