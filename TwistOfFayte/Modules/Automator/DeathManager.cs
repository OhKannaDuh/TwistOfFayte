using System;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Ocelot.Chain;
using Ocelot.Chain.Steps;
using Ocelot.Lifecycle;
using Ocelot.Services.Logger;
using TwistOfFayte.Config;
using TwistOfFayte.Services.State;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace TwistOfFayte.Modules.Automator;

public class DeathManager(
    IAddonLifecycle lifecycle,
    IStateManager state,
    DeathConfig config,
    IChainFactory chains,
    ICondition condition,
    ILogger logger
) : IOnStart, IOnStop, IOnUpdate
{
    private CancellationTokenSource autoRespawnCancel = new();

    private Task<ChainResult>? autoRespawnTask;

    private CancellationTokenSource autoAcceptRaiseToken = new();

    private Task<ChainResult>? autoAcceptRaiseTask;


    public void OnStart()
    {
        logger.Info("Registering Yesno lifecycle event");
        lifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", OnSelectYesnoPostSetup);
    }

    public void OnStop()
    {
        logger.Info("Unregistering Yesno lifecycle event");
        lifecycle.UnregisterListener(AddonEvent.PostSetup, "SelectYesno", OnSelectYesnoPostSetup);
    }

    public void Update()
    {
        if (autoRespawnTask is { IsCompleted: true })
        {
            logger.Info("Disposing of AutoRespawn");
            autoRespawnTask.Dispose();
            autoRespawnTask = null;
        }

        if (autoAcceptRaiseTask == null)
        {
            return;
        }

        if (autoRespawnTask != null)
        {
            autoRespawnCancel.Cancel();
        }

        if (autoAcceptRaiseTask.IsCompleted)
        {
            autoAcceptRaiseTask.Dispose();
            autoAcceptRaiseTask = null;
        }
    }

    private unsafe void OnSelectYesnoPostSetup(AddonEvent type, AddonArgs args)
    {
        if (!condition[ConditionFlag.Unconscious] || !state.IsActive())
        {
            return;
        }

        var addon = (AtkUnitBase*)args.Addon.Address;
        if (addon == null || !addon->IsVisible)
        {
            return;
        }

        if (config.ShouldAutoRespawn && addon->AtkValues[4].Type == ValueType.Int && addon->AtkValues[4].Int == 1)
        {
            HandleRespawnAddon(args);
        }

        // @todo investigate raise atk values
        // if (config.ShouldAcceptRaises && addon->AtkValues[4].Type == ValueType.Int && addon->AtkValues[4].Int == 1)
        // {
        //     HandleRaiseAddon(args);
        // }
    }

    private void HandleRespawnAddon(AddonArgs args)
    {
        if (autoRespawnTask != null)
        {
            return;
        }

        logger.Info("Kicking off Auto Release Task");
        autoRespawnCancel = new CancellationTokenSource();
        autoRespawnTask = chains.Create("AutoRelease")
            .Then(new ActionStep(async context =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(config.AutoRespawnDelay), context.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                    logger.Info("Cancel requested during delay");
                    return StepResult.Break();
                }

                if (context.CancellationToken.IsCancellationRequested)
                {
                    logger.Info("Cancel requested after delay");
                    return StepResult.Break();
                }

                unsafe
                {
                    var addon = (AtkUnitBase*)args.Addon.Address;
                    if (addon == null || !addon->IsVisible)
                    {
                        logger.Info("Addon is no longer visible");
                        return StepResult.Failure("Addon is no longer visible");
                    }


                    logger.Info("firing callback");
                    addon->FireCallbackInt(0);
                }

                return StepResult.Success();
            }, "AutoRelease::Delay"))
            .ExecuteAsync(autoRespawnCancel.Token);
    }

    private void HandleRaiseAddon(AddonArgs args)
    {
        if (autoAcceptRaiseTask != null)
        {
            return;
        }

        logger.Info("Kicking off Auto Raise Task");
        autoAcceptRaiseToken = new CancellationTokenSource();
        autoAcceptRaiseTask = chains.Create("AutoRaise")
            .Then(new ActionStep(async context =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(config.AcceptRaiseDelay), context.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return StepResult.Break();
                }

                if (context.CancellationToken.IsCancellationRequested)
                {
                    return StepResult.Break();
                }

                unsafe
                {
                    var addon = (AtkUnitBase*)args.Addon.Address;
                    if (addon == null || !addon->IsVisible)
                    {
                        return StepResult.Failure("Addon is no longer visible");
                    }

                    addon->FireCallbackInt(0);
                }

                return StepResult.Break();
            }, "AutoRaise::Delay"))
            .ExecuteAsync(autoAcceptRaiseToken.Token);
    }
}
