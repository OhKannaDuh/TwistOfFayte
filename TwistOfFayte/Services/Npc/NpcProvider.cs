using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.ObjectLifeTracker;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Data;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Npc;

public class NpcProvider(
    IObjectTable objects,
    IPlayer player,
    IStateManager state,
    CombatConfig config,
    INpcRangeProvider ranges
) : INpcProvider, IOnPreUpdate
{
    private static IEnumerable<Target> Npcs { get; set; } = [];

    private static IEnumerable<Target> NonFateNpcs { get; set; } = [];

    public Target? GetFateStartNpc()
    {
        return Npcs.FirstOrDefault(t => t is { IsHostile: false, NamePlateIconId: 60093 });
    }

    public Target? GetFateTurnInNpc()
    {
        return Npcs.FirstOrDefault(t => t is { IsHostile: false, NamePlateIconId: 60732 });
    }

    public IEnumerable<Target> GetEnemies()
    {
        return Npcs.Where(o => o is { IsHostile: true, IsTargetable: true, IsDead: false });
    }

    public IEnumerable<Target> GetRangedEnemies()
    {
        return GetEnemies().Where(t => t.IsRanged);
    }

    public IEnumerable<Target> GetMeleeEnemies()
    {
        return GetEnemies().Where(t => t.IsMelee);
    }

    public IEnumerable<Target> GetForlornMaidens()
    {
        return GetEnemies().Where(t => t.NameId is 6737 or 6738);
    }

    public IEnumerable<Target> GetNonFateNpcs()
    {
        return NonFateNpcs;
    }

    public void PreUpdate()
    {
        NonFateNpcs = objects.OfType<IBattleNpc>()
            .Where(o => o is
            {
                IsDead: false,
                IsTargetable: true,
            })
            .Select(o => new Target(o, ranges.GetRange(o)))
            .ToList();

        Npcs = objects.OfType<IBattleNpc>()
            .Where(o => o is
            {
                IsDead: false,
                IsTargetable: true,
            })
            .Where(IsTargetForSelectedFate)
            .Where(o => o.GetLifeTimeSeconds() >= config.MobLifetimeSecondsRequirement)
            .OrderBy(o => Vector3.Distance(o.Position, player.GetPosition()))
            .Select(o => new Target(o, ranges.GetRange(o)))
            .ToList();
    }

    private unsafe bool IsTargetForSelectedFate(IBattleNpc npc)
    {
        if (!state.IsActive())
        {
            return false;
        }

        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            return false;
        }


        var battleChara = (BattleChara*)npc.Address;

        return battleChara->FateId == selected.Value.Value;
    }
}
