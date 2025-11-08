using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("other")]
public class UXConfig : IAutoConfig
{
    [Checkbox] public bool HighlightMobs { get; set; } = true;
}
