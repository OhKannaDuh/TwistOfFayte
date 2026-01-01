using System.Threading;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using Ocelot.Chain;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.UI;
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
    IObjectTable objects,
    CombatConfig combat,
    IUIService ui
) : BaseHandler(ParticipatingInFateState.FightGatheredMobs, state, fates, npcs, objects, combat)
{
    private Task<ChainResult>? RepositionTask;

    private readonly CancellationTokenSource cancel = new();

    private readonly IObjectTable objects = objects;

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
            targeter.GetTarget()?.TryUse((in t) => targetManager.Target = t.GameObject, objects);
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

    public override void Render()
    {
        ui.LabelledValue("Gathered Count", GatheredCount);
        ui.LabelledValue("Goal", Goal);
        ui.LabelledValue("Candidate Count", CandidatesCount);

        foreach (var candidate in GetCandidates())
        {
            ui.Text(candidate.NameId);
        }

        foreach (var e in GetEnemies())
        {
            ui.Text(e.ObjectId);
            ImGui.Indent();
            e.TryUse(static (in t) => t.IsTargetingAnyPlayer(), objects, out var a);
            ui.LabelledValue("Is Targeting Any Player: ", a);

            e.TryUse(static (in t) => t.IsTargetingLocalPlayer(), objects, out var b);
            ui.LabelledValue("Is Targeting Local Player: ", b);

            e.TryUse(static (in t) => t.GetTargetedPlayer()?.HasTankStanceOn() == false, objects, out var c);
            ui.LabelledValue("Is Targeting other player with stance: ", c);
            ImGui.Unindent();
        }
    }
}
