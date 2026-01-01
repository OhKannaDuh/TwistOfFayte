using Dalamud.Game.ClientState.Objects.Types;

namespace TwistOfFayte.Services.Npc;

public interface INpcRangeProvider
{
    float GetRange(IBattleNpc npc);
}
