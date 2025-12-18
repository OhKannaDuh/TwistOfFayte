using System.Linq;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Targeter;

public class ClosestToPlayerTargeter(
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
        if (EzThrottler.Throttle("ClosestToPlayerTargeter::Change"))
        {
            return targetManager.Target?.Address == GetTarget()?.GameObject.Address;
        }

        return false;
    }

    public Target? GetTarget()
    {
        // Closest enemy to player
        return GetNpcsTargetingLocalPlayer().FirstOrDefault();
    }

    public bool Contains(Target target)
    {
        var range = player.IsHealer() ? 8f : 5f;
        var origin = player.GetPosition();

        return origin.Distance(target.Position) - target.GameObject.HitboxRadius <= range;
    }

    public string Identify()
    {
        return "Closest To Player";
    }
}
