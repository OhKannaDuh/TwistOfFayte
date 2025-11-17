using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Ocelot;
using Ocelot.Chain.Services;
using Ocelot.Config;
using Ocelot.ECommons.Services;
using Ocelot.Extensions;
using Ocelot.Mechanic.Services;
using Ocelot.Pathfinding.Services;
using Ocelot.Pictomancy.Services;
using Ocelot.Rotation.Services;
using Ocelot.Services;
using Ocelot.Services.Commands;
using Ocelot.Services.Translation;
using Ocelot.Services.WindowManager;
using Ocelot.States;
using Ocelot.UI.Services;
using Ocelot.Windows;
using TwistOfFayte.Chains.Steps;
using TwistOfFayte.Commands;
using TwistOfFayte.Config;
using TwistOfFayte.Config.Excel;
using TwistOfFayte.Data;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Modules.Automator;
using TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate;
using TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;
using TwistOfFayte.Modules.Automator.Handlers.TravellingToFate;
using TwistOfFayte.Modules.Debug;
using TwistOfFayte.Modules.Translations;
using TwistOfFayte.Modules.Ux;
using TwistOfFayte.Renderers;
using TwistOfFayte.Renderers.Help;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Fates.CombatHelper.Positioner;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;
using TwistOfFayte.Services.Materia;
using TwistOfFayte.Services.Materia.Steps;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.Repair;
using TwistOfFayte.Services.Repair.Steps;
using TwistOfFayte.Services.State;
using TwistOfFayte.Services.Zone;
using TwistOfFayte.Windows;
using ConfigCommand = TwistOfFayte.Commands.ConfigCommand;
using IConfiguration = TwistOfFayte.Config.IConfiguration;
using MainCommand = TwistOfFayte.Commands.MainCommand;

namespace TwistOfFayte;

public sealed class Plugin(IDalamudPluginInterface plugin) : OcelotPlugin(plugin)
{
    private readonly IDalamudPluginInterface plugin = plugin;

    public override string Name
    {
        get => "Twist of Fayte";
    }

    protected override void Boostrap(IServiceCollection services)
    {
        BootstrapOcelotModules(services);
        BootstrapCommands(services);
        BootstrapConfiguration(services, plugin);
        BootstrapServices(services);
        BootstrapModules(services);
        BootstrapRenderers(services);
        BootstrapTranslationContext(services);
    }

    private static void BootstrapOcelotModules(IServiceCollection services)
    {
        services.LoadECommons();
        services.LoadPictomancy();
        services.LoadPathfinding();
        services.LoadMechanics();
        services.LoadRotations();
        services.LoadChain();
        services.LoadUI();
    }

    private static void BootstrapCommands(IServiceCollection services)
    {
        services.AddSingleton<IMainCommand, MainCommand>();
        services.AddSingleton<IConfigCommand, ConfigCommand>();
    }

    private static void BootstrapConfiguration(IServiceCollection services, IDalamudPluginInterface plugin)
    {
        services.AddSingleton<MountDisplay>();
        services.AddSingleton<MountFilter>();

        services.AddSingleton<ZoneDisplay>();
        services.AddSingleton<ZoneFilter>();

        services.AddConfig<Configuration, IConfiguration>(plugin);
    }

    private static void BootstrapServices(IServiceCollection services)
    {
        services.AddSingleton<IFateRepository, FateRepository>();
        services.AddSingleton<IFateScorer, FateScorer>();
        services.AddSingleton<IFateSelector, ScoreBasedFateSelector>();

        services.AddSingleton<IStateManager, StateManager>();
        services.AddSingleton<IZone, Zone>();
        services.AddSingleton<INpcProvider, NpcProvider>();
        services.AddSingleton<INpcRangeProvider, NpcRangeProvider>();

        services.AddSingleton<IFateFactory, FateFactory>();

        services.AddSingleton<SingleTargetPositioner>();
        services.AddSingleton<CircularAoePositioner>();
        services.AddSingleton<CasterAoePositioner>();
        services.AddSingleton<IPositioner, DynamicPositioner>();

        services.AddSingleton<ClosestToPlayerTargeter>();
        services.AddSingleton<CentroidTargeter>();
        services.AddSingleton<ITargeter, DynamicTargeter>();

        services.AddSingleton<PlayerTracker>();

        services.AddTransient<UnmountStep>();

        services.AddSingleton<IRepairService, RepairService>();
        services.AddTransient<RepairStep>();

        services.AddSingleton<IMateriaExtractionService, MateriaExtractionService>();
        services.AddTransient<ExtractStep>();
    }

    private static void BootstrapModules(IServiceCollection services)
    {
        services.AddSingleton<TranslationLoader>();

        services.AddSingleton<AutomatorModule>();
        services.AddFlowStateMachine(AutomatorState.WaitingForFate);

        services.AddSingleton<TravellingToFateContext>();
        services.AddFlowStateMachine(TravellingToFateState.ChoosingPath, ServiceLifetime.Transient);

        services.AddScoreStateMachine<ParticipatingInFateState, StatePriority>(ParticipatingInFateState.Entrance, ServiceLifetime.Transient);

        services.AddSingleton<DeathManager>();

        services.AddSingleton<UxModule>();

        services.AddSingleton<DebugModule>();

#if DEBUG
        services.AddSingleton<OpenWindows>();
#endif
    }

    private static void BootstrapRenderers(IServiceCollection services)
    {
        services.AddSingleton<IMainRenderer, RenderComposer>();
        services.AddSingleton<FateListRenderer>();
        services.AddSingleton<AutomationStateRenderer>();

        services.AddSingleton<DebugRenderer>();
        services.AddSingleton<IDebugRenderable, DynamicServiceDebugRenderable>();
        services.AddSingleton<IDebugRenderable, ConditionDebugRenderable>();

        services.AddSingleton<IConfigRenderer, ConfigRenderer>();

        services.AddSingleton<IHelpRenderer, HelpRenderer>();
        services.AddSingleton<HelpWindow>();
        services.AddSingleton<IWindow>(sp => sp.GetRequiredService<HelpWindow>());
        services.AddSingleton<HelpCommand>();
        services.AddSingleton<IMainCommandDelegate, HelpCommandDelegate>();
    }

    private void BootstrapTranslationContext(IServiceCollection services)
    {
        services.AddSingleton(new TranslatorContextResolverOptions(GetType())
        {
            Replacements = new Dictionary<Regex, Func<Type, string>>
            {
                { new Regex(@"^TwistOfFayte\.Renderers\..*"), type => $"renderers.{type.Name.Replace("Renderer", "").ToSnakeCase()}" },
            },
        });
    }
}
