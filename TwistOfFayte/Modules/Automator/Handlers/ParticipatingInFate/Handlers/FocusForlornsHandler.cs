using System;
using System.Linq;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Throttlers;
using Ocelot.Extensions;
using Ocelot.Pathfinding.Extensions;
using Ocelot.Rotation.Services;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class FocusForlornsHandler(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    CombatConfig combat,
    ITargetManager targetManager,
    IRotationService rotation,
    IPathfinder pathfinder,
    IPlayer player,
    CombatConfig config
) : BaseHandler(ParticipatingInFateState.FocusForlorns, state, fates, npcs, combat), IDisposable
{
    private readonly INpcProvider npcs = npcs;

    public override StatePriority GetScore()
    {
        if (!config.FocusForlorns)
        {
            return StatePriority.Never;
        }

        return npcs.GetForlornMaidens().Any() ? StatePriority.VeryHigh : StatePriority.Never;
    }

    public override void Enter()
    {
        base.Enter();
        rotation.EnableSingleTarget();
    }

    public override void Exit(ParticipatingInFateState next)
    {
        base.Exit(next);
        rotation.DisableSingleTarget();
    }

    public override void Handle()
    {
        var maiden = npcs.GetForlornMaidens().FirstOrNull();
        if (maiden == null)
        {
            return;
        }

        if (EzThrottler.Throttle("TargetForlorn"))
        {
            maiden.Value.TryUse((in t) => targetManager.Target = t.GameObject);
        }

        var distance = player.GetPosition().Distance2D(maiden.Value.Position);
        if (pathfinder.IsIdle() && distance > player.GetAttackRange())
        {
            pathfinder.PathfindAndMoveTo(new PathfinderConfig(maiden.Value.GetApproachPosition(player.GetPosition(), player.GetAttackRange())));
        }
    }

    public void Dispose()
    {
        rotation.Unload();
    }
}
