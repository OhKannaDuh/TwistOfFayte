using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("general")]
public class GeneralConfig : IAutoConfig
{
    [Checkbox] public bool ShouldAutoRepair { get; set; } = true;

    [IntRange(0, 99)] public int AutoRepairThreshold { get; set; } = 30;

    [Checkbox] public bool ShouldAutoExtractMateria { get; set; } = true;
}
