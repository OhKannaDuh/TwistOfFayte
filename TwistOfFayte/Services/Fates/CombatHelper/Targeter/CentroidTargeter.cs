using System.Linq;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Targeter;

public class CentroidTargeter(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player,
    ITargetManager targetManager
) : BaseCombatHelper(state, fates, npcs, player), ITargeter
{
    private readonly IPlayer player = player;

    public bool ShouldChange()
    {
        if (EzThrottler.Throttle("CentroidTargeter::Change"))
        {
            return targetManager.Target?.Address == GetTarget()?.GameObject.Address;
        }

        return false;
    }

    public Target? GetTarget()
    {
        var npcs = GetNpcsTargetingLocalPlayer().ToList();
        if (npcs.Count == 0)
        {
            return null;
        }

        var centroid = npcs.Select(npc => npc.Position).Centroid();
        return npcs.OrderBy(npc => npc.Position.Truncate().Distance(centroid)).First();
    }


    public bool Contains(Target target)
    {
        if (targetManager.Target == null)
        {
            return false;
        }

        var range = player.IsHealer() ? 8f : 5f;
        var origin = targetManager.Target.Position;

        return origin.Distance(target.Position) <= range;
    }

    public string Identify()
    {
        return "Centroid";
    }
}
