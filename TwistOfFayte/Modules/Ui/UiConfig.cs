namespace TwistOfFayte.Modules.Ui;

public class UiConfig : ModuleConfig
{
    [Checkbox]
    public bool ShowTimeEstimate { get; set; } = true;

    [Checkbox]
    public bool ShowObjectiveEstimate { get; set; } = true;

    [Checkbox]
    public bool ShowBonusFateIcon { get; set; } = true;

    [Checkbox]
    public bool HighlightSelectedFate { get; set; } = true;

    [Checkbox]
    public bool ShowPreparingFateIcon { get; set; } = true;
}
