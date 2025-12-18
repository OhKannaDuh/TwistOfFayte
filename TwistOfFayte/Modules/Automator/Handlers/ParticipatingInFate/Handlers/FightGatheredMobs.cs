using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using Ocelot.Chain;
using Ocelot.Services.Pathfinding;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Fates.CombatHelper.Positioner;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class FightGatheredMobsHandler(
    ITargetManager targetManager,
    IPositioner positioner,
    ITargeter targeter,
    IPathfinder pathfinder,
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    CombatConfig combat
) : BaseHandler(ParticipatingInFateState.FightGatheredMobs, state, fates, npcs, combat)
{
    private Task<ChainResult>? RepositionTask;

    private readonly CancellationTokenSource cancel = new();

    public override void Exit(ParticipatingInFateState next)
    {
        base.Exit(next);

        cancel.Cancel();
        if (RepositionTask?.IsCompleted ?? false)
        {
            RepositionTask?.Dispose();
        }

        RepositionTask = null;
    }

    public override StatePriority GetScore()
    {
        var fate = GetFate();
        if (fate == null)
        {
            return StatePriority.Never;
        }

        if (GatheredCount >= Goal)
        {
            return StatePriority.High;
        }

        return GatheredCount > 0 && CandidatesCount <= 0 ? StatePriority.High : StatePriority.Never;
    }

    public override void Handle()
    {
        if (!EzThrottler.Throttle("FightGatheredMobsHandler::Gate"))
        {
            return;
        }

        if (RepositionTask != null)
        {
            if (RepositionTask.IsCompleted)
            {
                RepositionTask = null;
            }

            return;
        }

        if (targeter.ShouldChange() || targetManager.Target == null)
        {
            targetManager.Target = targeter.GetTarget()?.GameObject;
        }

        if (positioner.ShouldMove() && pathfinder.GetState() == PathfindingState.Idle)
        {
            pathfinder.PathfindAndMoveTo(new PathfinderConfig(positioner.GetPosition()));
        }

        if (!positioner.ShouldMove())
        {
            pathfinder.Stop();
        }
    }
}
