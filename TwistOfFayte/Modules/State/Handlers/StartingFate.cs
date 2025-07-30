using System;
using System.Linq;
using Dalamud.Game.ClientState.Fates;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.StartingFate)]
public class StartingFate(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override unsafe State? Handle()
    {
        if (FateHelper.SelectedFate == null)
        {
            return State.Idle;
        }

        if (FateHelper.SelectedFate.State != FateState.Preparation || !TargetHelper.Friendlies.Any())
        {
            return State.ParticipatingInFate;
        }

        if (Player.Mounted)
        {
            Actions.TryUnmount();
        }

        var npc = TargetHelper.Friendlies.First();
        Svc.Targets.Target = npc;
        if (Player.DistanceTo(npc) > 5f && !Module.VNavmesh.IsRunning())
        {
            Module.VNavmesh.PathfindAndMoveTo(npc.Position, false);
        }

        if (Player.DistanceTo(npc) <= 5f && Module.VNavmesh.IsRunning())
        {
            Module.VNavmesh.Stop();
        }

        if (!Plugin.Chain.IsRunning)
        {
            Plugin.Chain.Submit(() =>
                Chain.Create("StartingFate.InteractWithNpc")
                    .BreakIf(() => Svc.Targets.Target == null)
                    .Debug("Target was not null, waiting to not be mounted")
                    .Then(_ => !Player.Mounted)
                    .Debug("Player is not mounted, interacting with npc")
                    .Then(_ => TargetSystem.Instance()->InteractWithObject(Svc.Targets.Target.Struct(), false) != 0)
                    .Debug("Waiting for Talk Addon")
                    .WaitForAddonReady("Talk", 200)
                    .Debug("Waiting for talking to be done")
                    .Then(_ => {
                        if (!EzThrottler.Throttle("StartingFate.InteractWithNpc.Talk", 100))
                        {
                            return false;
                        }

                        var addonPtr = Svc.GameGui.GetAddonByName("Talk");
                        if (addonPtr == IntPtr.Zero)
                        {
                            return true;
                        }

                        var addon = (AtkUnitBase*)addonPtr;
                        if (!GenericHelpers.IsAddonReady(addon))
                        {
                            return true;
                        }

                        new AddonMaster.Talk(addonPtr).Click();
                        return false;
                    })
                    .Debug("Waiting for yes no to appear or to be in fate")
                    .Then(_ => {
                        // wait for yesno to be ready
                        var addonPtr = Svc.GameGui.GetAddonByName("SelectYesno");
                        if (addonPtr != IntPtr.Zero)
                        {
                            var addon = (AtkUnitBase*)addonPtr;
                            new AddonMaster.SelectYesno(addon).Yes();
                            return true;
                        }

                        if (FateHelper.IsInFate())
                        {
                            Svc.Log.Warning("IN FATE");
                        }

                        // or in fate
                        return FateHelper.IsInFate();
                    })
            );
        }

        return null;
    }
}
