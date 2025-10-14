using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot;
using Ocelot.Config;
using Ocelot.ECommons.Services;
using Ocelot.Services;
using Ocelot.Services.WindowManager;
using Ocelot.UI.Services;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Modules.StartUp;
using TwistOfFayte.Renderers;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.State;
using TwistOfFayte.Services.Zone;
using IConfiguration = TwistOfFayte.Config.IConfiguration;

namespace TwistOfFayte;

public sealed class Plugin(IDalamudPluginInterface plugin, IFateTable fates) : OcelotPlugin(plugin)
{
    private readonly IDalamudPluginInterface plugin = plugin;

    protected override void Boostrap(IServiceCollection services)
    {
        services.AddSingleton(fates);

        BootstrapOcelotModules(services);
        BootstrapConfiguration(services, plugin);
        BootstrapServices(services);
        BootstrapRenderers(services);

        services.AddSingleton<OpenWindows>();
    }

    private static void BootstrapOcelotModules(IServiceCollection services)
    {
        services.LoadECommons();
        // services.LoadPictomancy();
        // services.LoadPathfinding();
        // services.LoadMechanics();
        // services.LoadRotations();
        // services.LoadChain();
        services.LoadUI();
    }

    private static void BootstrapConfiguration(IServiceCollection services, IDalamudPluginInterface plugin)
    {
        services.AddSingleton(plugin.GetPluginConfig() as Configuration ?? new Configuration());
        services.AddSingleton<IConfiguration>(s => s.GetRequiredService<Configuration>());
        services.AddSingleton<IPluginConfiguration>(s => s.GetRequiredService<Configuration>());

        services.AddConfig<ScorerConfig, IConfiguration>(config => config.ScorerConfig);
        services.AddConfig<TraversalConfig, IConfiguration>(config => config.TraversalConfig);
        services.AddConfig<UIConfig, IConfiguration>(config => config.UIConfig);
    }

    private static void BootstrapServices(IServiceCollection services)
    {
        services.AddSingleton<IFateRepository, FateRepository>();
        services.AddSingleton<IFateScorer, FateScorer>();
        services.AddSingleton<IFateSelector, ScoreBasedFateSelector>();

        services.AddSingleton<IStateManager, StateManager>();
        services.AddSingleton<IZone, Zone>();

        services.AddSingleton<IFateFactory, FateFactory>();
    }

    private static void BootstrapRenderers(IServiceCollection services)
    {
        services.AddSingleton<IMainRenderer, RenderComposer>();
        services.AddSingleton<FateListRenderer>();

        services.AddSingleton<IConfigRenderer, ConfigRenderer>();
    }
}
