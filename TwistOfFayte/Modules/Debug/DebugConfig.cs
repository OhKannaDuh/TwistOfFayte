namespace TwistOfFayte.Modules.Debug;

public class DebugConfig : ModuleConfig
{
    [Checkbox]
    public bool RenderDistanceToEngagedEnemies { get; set; } = false;

    [Checkbox]
    public bool RenderDistanceToNotEngagedEnemies { get; set; } = false;

    [Checkbox]
    public bool RenderAoeRadiusAroundPlayer { get; set; } = false;

    [Checkbox]
    public bool RenderLinetoEngagedEnemiesOutOfAoeRadius { get; set; } = false;

    [Checkbox]
    public bool RenderLineToNonEngagedEnemies { get; set; } = false;
}
