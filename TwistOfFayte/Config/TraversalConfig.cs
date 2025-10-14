using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
public class TraversalConfig : IAutoConfig
{
    [FloatRange(6f, 20f)] public float CostPerYalm { get; set; } = 20f;

    [IntRange(5, 10)] public int TimeToTeleport { get; set; } = 6;

    [Checkbox] public bool ShouldTeleport { get; set; } = true;
}
