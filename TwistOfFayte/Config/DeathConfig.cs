using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_participation")]
public class DeathConfig : IAutoConfig
{
    [Checkbox] public bool ShouldAutoRespawn { get; set; } = true;

    [IntRange(0, 300)] public int AutoRespawnDelay { get; set; } = 15;

    [Checkbox] public bool ShouldAcceptRaises { get; set; } = true;

    [IntRange(0, 300)] public int AcceptRaiseDelay { get; set; } = 3;
}
