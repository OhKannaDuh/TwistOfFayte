using System;
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
        foreach (var e in GetEnemies())
        {
            if (e.TryUse(static (in t) => t.IsTargetingLocalPlayer(), out var result) && result)
            {
                yield return e;
            }
        }
    }

    protected IEnumerable<Target> GetEnemiesTargetingNoPlayer()
    {
        foreach (var e in GetEnemies())
        {
            if (e.TryUse(static (in t) => t.IsTargetingAnyPlayer(), out var result) && result)
            {
                yield return e;
            }
        }
    }

    protected IEnumerable<Target> GetEnemiesTargetingPlayersWithoutTankStance()
    {
        foreach (var e in GetEnemies())
        {
            // I hate this...
            if (e.TryUse(static (in t) => t.IsTargetingAnyPlayer()
                                          && !t.IsTargetingLocalPlayer()
                                          && t.GetTargetedPlayer()?.HasTankStanceOn() == false, out var result) && result)
            {
                yield return e;
            }
        }
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
        get
        {
            var fate = GetFate();
            if (fate == null)
            {
                return combat.GetMaxMobsToFight();
            }

            var remaining = fate.ObjectiveTracker.ObjectivesRemaining;
            return remaining != -1 ? Math.Min(remaining, combat.GetMaxMobsToFight()) : combat.GetMaxMobsToFight();
        }
    }
}
