using System.Linq;
using Dalamud.Game.ClientState.Objects;
using ECommons.Throttlers;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Ocelot.Services.UI;
using TwistOfFayte.Config;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class GatherMobsHandler(
    IPlayer player,
    ITargetManager targetManager,
    IPathfinder pathfinder,
    IUIService ui,
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    CombatConfig combat
) : BaseHandler(ParticipatingInFateState.GatherMobs, state, fates, npcs, combat)
{
    private Target? candidate;

    public override StatePriority GetScore()
    {
        var fate = GetFate();
        if (fate == null || GatheredCount >= Goal)
        {
            return StatePriority.Never;
        }

        return CandidatesCount <= 0 ? StatePriority.Never : StatePriority.High;
    }

    public override void Handle()
    {
        if (candidate == null)
        {
            candidate = GetCandidates().FirstOrDefault();
            return;
        }

        var candidatesTarget = candidate.GetTargetedPlayer();
        if (candidatesTarget != null && (candidatesTarget.IsLocalPlayer() || candidatesTarget.HasTankStanceOn()))
        {
            pathfinder.Stop();
            candidate = null;
            return;
        }

        var isTargetingCandidate = targetManager.Target?.Address == candidate.GameObject.Address;
        if (!isTargetingCandidate && EzThrottler.Throttle("Target"))
        {
            targetManager.Target = candidate.GameObject;
            return;
        }

        if (pathfinder.GetState() == PathfindingState.Idle && !player.IsCasting())
        {
            pathfinder.PathfindAndMoveTo(new PathfinderConfig(candidate.GetApproachPosition(player.GetPosition(), player.GetAttackRange())));
        }
    }

    public override void Render()
    {
        ui.LabelledValue("Gathered Count", GatheredCount);
        ui.LabelledValue("Goal", Goal);
        ui.LabelledValue("Candidate Count", CandidatesCount);
    }
}
