using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("fate_selection")]
public class ScorerConfig : IAutoConfig
{
    [FloatRange(-1024f, 1024f)] public float BonusFateModifier { get; set; } = 512f;

    [FloatRange(-1024f, 1024f)] public float UnstartedFateModifier { get; set; } = 128f;

    [FloatRange(-512f, 512f)] public float MobFateModifier { get; set; } = 0f;

    [FloatRange(-512f, 512f)] public float BossFateModifier { get; set; } = 0f;

    [FloatRange(-512f, 512f)] public float CollectFateModifier { get; set; } = 0f;

    [FloatRange(-512f, 512f)] public float DefendFateModifier { get; set; } = 0f;

    [FloatRange(-512f, 512f)] public float EscortFateModifier { get; set; } = 0f;


    [FloatRange(0f, 2f)] public float InProgressFateModifier { get; set; } = 2f;

    [IntRange(0, 180)] public int TimeRequiredToConsiderFate { get; set; } = 30;
}
