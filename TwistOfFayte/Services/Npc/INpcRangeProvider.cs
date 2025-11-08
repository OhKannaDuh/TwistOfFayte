using Dalamud.Game.ClientState.Objects.Types;

namespace TwistOfFayte.Services.Npc;

public interface INpcRangeProvider
{
    public float GetRange(IBattleNpc npc);
}
