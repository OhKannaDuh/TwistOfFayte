using System;
using Ocelot.Config;
using Ocelot.Config.Fields;
using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_selection")]
public class FateSelectorConfig : IAutoConfig
{
    [Checkbox] public bool DoMobsFates { get; set; } = true;

    [Checkbox] public bool DoBossFates { get; set; } = true;

    // [Checkbox] // Let's not show this option for now
    public bool DoCollectFates { get; set; } = false;

    [Checkbox] public bool DoDefendFates { get; set; } = true;

    [Checkbox] public bool DoEscortFates { get; set; } = true;

    internal bool ShouldDoFate(Fate fate)
    {
        return false;
        return fate.Type switch
        {
            FateType.Mobs => DoMobsFates,
            FateType.Boss => DoBossFates,
            FateType.Collect => DoCollectFates,
            FateType.Defend => DoDefendFates,
            FateType.Escort => DoEscortFates,
            _ => false,
        };
    }
}
