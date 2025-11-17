using Dalamud.Configuration;

namespace TwistOfFayte.Config;

public interface IConfiguration : IPluginConfiguration
{
    ScorerConfig ScorerConfig { get; set; }

    TraversalConfig TraversalConfig { get; set; }

    MultiZoneConfig MultiZoneConfig { get; set; }

    CombatConfig CombatConfig { get; set; }

    FateSelectorConfig FateSelectorConfig { get; set; }

    DeathConfig DeathConfig { get; set; }

    GeneralConfig GeneralConfig { get; set; }

    UIConfig UIConfig { get; set; }

    UXConfig UXConfig { get; set; }

    DebugConfig DebugConfig { get; set; }

    FateBlacklist FateBlacklist { get; set; }
}
