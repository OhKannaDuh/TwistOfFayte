using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Throttlers;
using Ocelot.Extensions;
using Ocelot.Pathfinding.Extensions;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Ocelot.States.Flow;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class InCombatHandler(
    IStateManager state,
    ICondition condition,
    INpcProvider npcs,
    ITargetManager targetManager,
    IPathfinder pathfinder,
    IPlayer player
) : FlowStateHandler<AutomatorState>(AutomatorState.InCombat)
{
    public override AutomatorState? Handle()
    {
        if (!state.IsActive())
        {
            return null;
        }

        if (!condition[ConditionFlag.InCombat])
        {
            return AutomatorState.WaitingForFate;
        }

        if (EzThrottler.Throttle("InCombat::Target"))
        {
            if (targetManager.Target == null)
            {
                var candidate = npcs.GetNonFateNpcs().Where(n => n.TryUse((in t) => t.IsTargetingLocalPlayer(), out var result) && result).FirstOrNull();
                candidate?.TryUse((in t) => targetManager.Target = t.GameObject);
            }

            if (targetManager.Target != null)
            {
                var range = player.GetAttackRange();
                var distance = targetManager.Target.Position.Truncate().Distance(player.GetPosition());
                if (distance > range && pathfinder.IsIdle())
                {
                    pathfinder.PathfindAndMoveTo(new PathfinderConfig(targetManager.Target.Position.GetApproachPosition(player.GetPosition(), range)));
                }
            }
        }

        return null;
    }
}
