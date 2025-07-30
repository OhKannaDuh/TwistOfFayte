namespace TwistOfFayte.Modules.General;

[OcelotModule]
public class GeneralModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public override GeneralConfig Config {
        get => PluginConfig.GeneralConfig;
    }
}
