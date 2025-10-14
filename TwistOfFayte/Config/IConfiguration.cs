using Dalamud.Configuration;

namespace TwistOfFayte.Config;

public interface IConfiguration : IPluginConfiguration
{
    ScorerConfig ScorerConfig { get; set; }

    TraversalConfig TraversalConfig { get; set; }

    UIConfig UIConfig { get; set; }
}
