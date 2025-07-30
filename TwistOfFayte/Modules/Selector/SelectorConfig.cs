namespace TwistOfFayte.Modules.Selector;

public class SelectorConfig : ModuleConfig
{
    [FloatRange(6f, 20f)]
    public float CostPerYalm { get; set; } = 20f;

    [IntRange(5, 10)]
    public int TimeToTeleport { get; set; } = 6;

    [FloatRange(-1024f, 1024f)]
    public float BonusFateModifier { get; set; } = 512f;

    [FloatRange(-1024f, 1024f)]
    public float UnstartedFateModifier { get; set; } = 128f;

    [FloatRange(0f, 60f)]
    public float TimeRequiredToConsiderFate { get; set; } = 30f;

    [FloatRange(0f, 2f)]
    public float InProgressFateModifier { get; set; } = 2f;
}
