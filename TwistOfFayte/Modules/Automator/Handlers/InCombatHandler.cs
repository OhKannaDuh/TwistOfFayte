using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin.Services;
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
    ITargetManager target,
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
            var candidates = npcs.GetNonFateNpcs().Where(t => t.IsTargetingLocalPlayer());

            target.Target ??= candidates.FirstOrDefault()?.GameObject;

            if (target.Target != null)
            {
                var range = player.GetAttackRange();
                var distance = target.Target.Position.Truncate().Distance(player.GetPosition());
                if (distance > range && pathfinder.IsIdle())
                {
                    pathfinder.PathfindAndMoveTo(new PathfinderConfig(target.Target.Position.GetApproachPosition(player.GetPosition(), range)));
                }
            }
        }

        return null;
    }
}
