using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using Ocelot.Extensions;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class MaintainFateZoneHandler(
    IPlayer player,
    IPathfinder pathfinder,
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IObjectTable objects,
    CombatConfig combat
) : BaseHandler(ParticipatingInFateState.MaintainFateZone, state, fates, npcs, objects, combat)
{
    private Vector3? TargetPosition;

    public override void Exit(ParticipatingInFateState next)
    {
        base.Exit(next);
        TargetPosition = null;
    }

    public override StatePriority GetScore()
    {
        var fate = GetFate();
        if (fate == null)
        {
            return StatePriority.Never;
        }

        var playerPosition = player.GetPosition();
        if (TargetPosition != null && playerPosition.Distance2D(TargetPosition.Value) >= 0.5f)
        {
            return StatePriority.Critical;
        }

        var radius = fate.Radius;
        var distance = fate.Position.Distance2D(playerPosition);
        var normalizedDistance = Math.Clamp(distance / radius, 0f, 1f);

        if (normalizedDistance >= 0.95f)
        {
            return StatePriority.VeryHigh;
        }

        return StatePriority.Never;
    }

    public override void Handle()
    {
        var fate = GetFate();
        if (fate == null)
        {
            return;
        }

        TargetPosition ??= fate.Position.GetApproachPosition(player.GetPosition(), fate.Radius / 5f);

        if (pathfinder.GetState() == PathfindingState.Idle)
        {
            pathfinder.PathfindAndMoveTo(new PathfinderConfig(TargetPosition.Value));
        }
    }
}
