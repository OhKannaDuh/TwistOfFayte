using System.Text.Json.Serialization;
using Dalamud.Plugin;

namespace TwistOfFayte.Config;

public class Configuration : IConfiguration
{
    public int Version { get; set; }

    public ScorerConfig ScorerConfig { get; set; } = new();

    public TraversalConfig TraversalConfig { get; set; } = new();

    public UIConfig UIConfig { get; set; } = new();
}
