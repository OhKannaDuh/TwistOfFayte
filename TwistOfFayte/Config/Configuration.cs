using Ocelot.Config;

namespace TwistOfFayte.Config;

public class Configuration : IConfiguration
{
    [ConfigHidden] public int Version { get; set; }

    public ScorerConfig ScorerConfig { get; set; } = new();

    public TraversalConfig TraversalConfig { get; set; } = new();

    public MultiZoneConfig MultiZoneConfig { get; set; } = new();

    public CombatConfig CombatConfig { get; set; } = new();

    public FateSelectorConfig FateSelectorConfig { get; set; } = new();

    public DeathConfig DeathConfig { get; set; } = new();

    public GeneralConfig GeneralConfig { get; set; } = new();

    public UIConfig UIConfig { get; set; } = new();

    public UXConfig UXConfig { get; set; } = new();

    public DebugConfig DebugConfig { get; set; } = new();

    public FateBlacklist FateBlacklist { get; set; } = new();
}
