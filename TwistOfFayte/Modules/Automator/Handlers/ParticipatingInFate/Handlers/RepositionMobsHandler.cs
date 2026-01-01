using Dalamud.Plugin.Services;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class RepositionMobsHandler(
    // ICombatHelper helper,
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IObjectTable objects,
    CombatConfig combat
) : BaseHandler(ParticipatingInFateState.RepositionMobs, state, fates, npcs, objects, combat)
{
    public override StatePriority GetScore()
    {
        return StatePriority.Never;
        // var fate = GetFate();
        // if (fate == null)
        // {
        //     return double.MinValue;
        // }
        //
        // return positioner.ShouldMove() ? 100d : double.MinValue;
    }

    public override void Handle()
    {
    }
}
