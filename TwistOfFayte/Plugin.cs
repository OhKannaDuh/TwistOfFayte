using Dalamud.Plugin;
using ECommons;

namespace TwistOfFayte;

public sealed class Plugin : OcelotPlugin
{
    public override string Name {
        get => "TwistOfFayte";
    }

    public Config Config { get; }

    public override IOcelotConfig OcelotConfig {
        get => Config;
    }

    public static ChainQueue Chain {
        get => ChainManager.Get("TwistOfFayte.Chain");
    }

    public Plugin(IDalamudPluginInterface plugin)
        : base(plugin, Module.DalamudReflector)
    {
        Config = plugin.GetPluginConfig() as Config ?? new Config();

        SetupLanguage(plugin);

        OcelotInitialize();

        ChainManager.Initialize();

#if DEBUG
        Windows.OpenMainUI();
        // Windows.OpenConfigUI();
#endif
    }

    private void SetupLanguage(IDalamudPluginInterface plugin)
    {
        I18N.SetDirectory(plugin.AssemblyLocation.Directory?.FullName!);
        I18N.LoadAllFromDirectory("en", "Translations/en");

        I18N.SetLanguage("en");
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
