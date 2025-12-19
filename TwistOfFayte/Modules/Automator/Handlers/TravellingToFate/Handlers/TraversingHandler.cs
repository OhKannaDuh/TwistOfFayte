using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Fates;
using ECommons;
using Ocelot.Extensions;
using Ocelot.Pathfinding.Extensions;
using Ocelot.Services.Logger;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Ocelot.States.Flow;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class TraversingHandler(
    IPlayer player,
    IPathfinder pathfinder,
    IStateManager state,
    TravellingToFateContext context,
    INpcProvider npcs,
    ILogger<TraversingHandler> logger
) : FlowStateHandler<TravellingToFateState>(TravellingToFateState.Traversing)
{
    private Vector3 destination;

    private Task? repathTask;

    public override void Enter()
    {
        base.Enter();

        if (context.fate == null)
        {
            logger.Error("No fate in context");
            return;
        }

        destination = context.fate.Position;
    }

    public override TravellingToFateState? Handle()
    {
        if (!state.IsActive() || context.chosenPath == null)
        {
            return TravellingToFateState.Arrived;
        }

        if (pathfinder.IsIdle())
        {
            var path = context.chosenPath.Smoothed().From(player.GetPosition());
            pathfinder.FollowPath(path);
        }

        // The goal here is to have all players path to a start npc if a start npc is required.
        // Melees (Tanks/MeleeDPS) should path to the closest enemy
        // Ranged (Casters/PhysRanged/Healers) should path to the auto 10% of the fate radius
        if (repathTask == null)
        {
            var startNpc = npcs.GetFateStartNpc();
            if (context.fate?.State == FateState.Preparation && startNpc != null)
            {
                repathTask = RepathToNpc(startNpc.Value);
            }

            if (player.IsMelee())
            {
                var enemy = npcs.GetEnemies().OrderBy(t => t.Position.Truncate().Distance(player.GetPosition())).FirstOrNull();
                if (context.fate?.State != FateState.Preparation && enemy != null)
                {
                    repathTask = RepathToNpc(enemy.Value);
                }
            }
            else if (context.fate != null && context.fate.State != FateState.Preparation)
            {
                // @todo sometimes a fate boundary extends off of a cliff
                var targetDistance = context.fate.Radius * 0.9f;
                repathTask = RepathToPoint(context.fate.Position, targetDistance);
            }
        }

        var distance = Vector3.Distance(destination, player.GetPosition());

        return distance <= 1f ? TravellingToFateState.Arrived : null;
    }

    private async Task RepathToNpc(Target npc)
    {
        destination = npc.GetApproachPosition(player.GetPosition());
        var path = await pathfinder.Pathfind(new PathfinderConfig(destination)
        {
            AllowFlying = player.CanFly(),
        });

        context.SetChosenPath(path);
        pathfinder.Stop();
    }

    private async Task RepathToPoint(Vector3 point, float distance)
    {
        destination = point.GetApproachPosition(player.GetPosition(), distance);
        var path = await pathfinder.Pathfind(new PathfinderConfig(destination)
        {
            AllowFlying = player.CanFly(),
        });

        context.SetChosenPath(path);
        pathfinder.Stop();
    }
}
