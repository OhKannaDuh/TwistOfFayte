using System.Numerics;
using Dalamud.Plugin.Services;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public class CasterAoePositioner(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player,
    IObjectTable objects
) : BaseCombatHelper(state, fates, npcs, player, objects), IPositioner
{
    private readonly IPlayer player = player;

    private readonly INpcProvider npcs = npcs;

    private const float range = 5f;

    public bool ShouldMove()
    {
        return GetPosition().Distance2D(player.GetPosition()) >= 0.5f;
    }

    public Vector3 GetPosition()
    {
        return player.GetPosition();
    }

    public string Identify()
    {
        return "Caster Aoe";
    }
}
