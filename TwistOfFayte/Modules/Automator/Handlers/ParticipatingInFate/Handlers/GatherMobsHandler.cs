using System.Linq;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using Ocelot.Services.Logger;
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
    IObjectTable objects,
    CombatConfig combat,
    ILogger<GatherMobsHandler> logger
) : BaseHandler(ParticipatingInFateState.GatherMobs, state, fates, npcs, objects, combat)
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

        if (!candidate.Value.TryUse((in t) => t.GetTargetedPlayer(), objects, out var candidatesTarget))
        {
            return;
        }

        if (candidatesTarget != null && (candidatesTarget.IsLocalPlayer() || candidatesTarget.HasTankStanceOn()))
        {
            pathfinder.Stop();
            candidate = null;
            return;
        }

        if (EzThrottler.Throttle("Target"))
        {
            if (!candidate.Value.TryUse((in t) => targetManager.Target?.Address == t.Address, objects, out var isTargetingCandidate))
            {
                return;
            }

            if (!isTargetingCandidate)
            {
                candidate.Value.TryUse((in t) => targetManager.Target = t.GameObject, objects);
            }

            return;
        }

        if (pathfinder.GetState() == PathfindingState.Idle && !player.IsCasting())
        {
            logger.Debug("Doing some pathginding...");
            pathfinder.PathfindAndMoveTo(new PathfinderConfig(candidate.Value.GetApproachPosition(player.GetPosition(), player.GetAttackRange())));
        }
    }

    public override void Render()
    {
        ui.LabelledValue("Gathered Count", GatheredCount);
        ui.LabelledValue("Goal", Goal);
        ui.LabelledValue("Candidate Count", CandidatesCount);
    }
}
