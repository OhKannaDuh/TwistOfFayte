using System.Collections.Generic;
using System.Linq;
using Ocelot.States.Score;
using TwistOfFayte.Config;
using TwistOfFayte.Data;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public abstract class BaseHandler(
    ParticipatingInFateState fateState,
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    CombatConfig combat
) : ScoreStateHandler<ParticipatingInFateState, StatePriority>(fateState)
{
    protected Fate? GetFate()
    {
        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            return null;
        }

        var snapshot = fates.Snapshot();

        return snapshot.FirstOrDefault(f => f.Id == selected.Value);
    }

    protected IEnumerable<Target> GetEnemies()
    {
        return npcs.GetEnemies();
    }

    protected IEnumerable<Target> GetEnemiesTargetingLocalPlayer()
    {
        return GetEnemies().Where(e => e.IsTargetingLocalPlayer());
    }

    protected IEnumerable<Target> GetEnemiesTargetingNoPlayer()
    {
        return GetEnemies().Where(e => !e.IsTargetingAnyPlayer());
    }

    protected IEnumerable<Target> GetEnemiesTargetingPlayersWithoutTankStance()
    {
        return GetEnemies().Where(e =>
            e.IsTargetingAnyPlayer() &&
            !e.IsTargetingLocalPlayer() &&
            e.GetTargetedPlayer()?.HasTankStanceOn() == false);
    }

    protected IEnumerable<Target> GetCandidates()
    {
        return GetEnemiesTargetingNoPlayer().Union(GetEnemiesTargetingPlayersWithoutTankStance());
    }

    protected int GatheredCount
    {
        get => GetEnemiesTargetingLocalPlayer().Count();
    }

    protected int CandidatesCount
    {
        get => GetCandidates().Count();
    }

    protected int Goal
    {
        get => combat.GetMaxMobsToFight();
    }
}
