using System.Collections.Generic;
using System.Linq;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Data;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Extensions;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper;

public abstract class BaseCombatHelper(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player
)
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

    protected IEnumerable<Target> GetNpcsTargetingLocalPlayer()
    {
        var position = player.GetPosition();
        return npcs.GetEnemies().WhereBattleTarget((in t) => t.IsTargetingLocalPlayer()).OrderBy(e => e.Position.Distance2D(position));
    }
}
