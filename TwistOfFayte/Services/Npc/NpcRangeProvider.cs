using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;

namespace TwistOfFayte.Services.Npc;

public class NpcRangeProvider : INpcRangeProvider
{
    private readonly Dictionary<uint, float> NpcRanges = new()
    {
        // Defective Sentry G6 (Heritage Found - It's Super Defective)
        { 13411, 9f },
        // Defective Sentry G6 (Heritage Found - Domo Arigato)
        { 13422, 9f },
        // Bandit Spellweaver (Koku - Tax Dodging)
        { 13014, 13f },

        { 8477, 19f },
    };


    public float GetRange(IBattleNpc npc)
    {
        return NpcRanges.TryGetValue(npc.NameId, out var range) ? range : 3.5f;
    }
}
