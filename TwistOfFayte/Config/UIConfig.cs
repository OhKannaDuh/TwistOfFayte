using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("other")]
public class UIConfig : IAutoConfig
{
    [Checkbox] public bool ShowTimeEstimate { get; set; } = true;

    [Checkbox] public bool ShowObjectiveEstimate { get; set; } = true;

    [Checkbox] public bool ShowFateTypeIcon { get; set; } = true;

    [Checkbox] public bool ShowBonusFateIcon { get; set; } = true;

    [Checkbox] public bool HighlightSelectedFate { get; set; } = true;

    [Checkbox] public bool FadeIgnoredFates { get; set; } = true;

    [Checkbox] public bool ShowPreparingFateIcon { get; set; } = true;
}
