using System;
using System.Linq;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Ocelot.Actions;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Ocelot.States.Score;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class TalkingToStarterNpcHandler(
    IStateManager state,
    IFateRepository fates,
    IPlayer player,
    IPathfinder pathfinder,
    INpcProvider npcs,
    ITargetManager targetManager,
    IObjectTable objects,
    IGameGui addons
) : ScoreStateHandler<ParticipatingInFateState, StatePriority>(ParticipatingInFateState.TalkingToStarterNpc)
{
    public override StatePriority GetScore()
    {
        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            return StatePriority.Never;
        }

        var snapshot = fates.Snapshot();
        var fate = snapshot.FirstOrDefault(f => f.Id == selected.Value);
        if (fate == null)
        {
            return StatePriority.Never;
        }

        return fate.State == FateState.Preparation ? StatePriority.Always : StatePriority.Never;
    }

    public override void Handle()
    {
        if (player.IsMounted() && Actions.Unmount.CanCast())
        {
            Actions.Unmount.Cast();
            pathfinder.Stop();
        }

        var target = npcs.GetFateStartNpc();
        if (target == null)
        {
            return;
        }

        if (targetManager.Target == null && EzThrottler.Throttle("Target"))
        {
            target.Value.TryUse((in t) => targetManager.Target = t.GameObject, objects);
        }

        if (targetManager.Target != null && !player.IsInteracting() && EzThrottler.Throttle("Interact"))
        {
            unsafe
            {
                var targetSystem = TargetSystem.Instance();
                if (targetSystem == null)
                {
                    return;
                }

                targetSystem->InteractWithObject(targetManager.Target.Struct(), false);
            }
        }

        // @todo: Consider catching this on addon lifecycle and skipping it when state is active
        if (EzThrottler.Throttle("Talk", 100))
        {
            var talkPtr = addons.GetAddonByName("Talk");
            if (talkPtr != IntPtr.Zero)
            {
                unsafe
                {
                    var talk = (AtkUnitBase*)talkPtr.Address;
                    if (talk->IsVisible && talk->UldManager.LoadedState == AtkLoadState.Loaded && talk->IsFullyLoaded())
                    {
                        new AddonMaster.Talk(talkPtr).Click();
                    }
                }
            }
        }

        if (EzThrottler.Throttle("SelectYesno"))
        {
            var selectYesNoPtr = addons.GetAddonByName("SelectYesno");
            if (selectYesNoPtr != IntPtr.Zero)
            {
                unsafe
                {
                    var selectYesNo = (AtkUnitBase*)selectYesNoPtr.Address;
                    new AddonMaster.SelectYesno(selectYesNo).Yes();
                }
            }
        }
    }
}
