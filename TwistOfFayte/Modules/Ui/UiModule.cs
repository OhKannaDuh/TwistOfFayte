namespace TwistOfFayte.Modules.Ui;

[OcelotModule]
public class UiModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public override UiConfig Config {
        get => PluginConfig.UiConfig;
    }
}
