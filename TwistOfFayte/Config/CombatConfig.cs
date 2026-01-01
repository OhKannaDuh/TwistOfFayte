using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_participation")]
public class CombatConfig : IAutoConfig
{
    [IntRange(0, 16)] public int MaxMobsToFight { get; set; } = 0;

    [IntRange(0, 10)] public int MobLifetimeSecondsRequirement { get; set; } = 2;

    [Checkbox] public bool FocusForlorns { get; set; } = true;
    
    public int GetMaxMobsToFight()
    {
        return MaxMobsToFight == 0 ? 16 : MaxMobsToFight;
    }
}
