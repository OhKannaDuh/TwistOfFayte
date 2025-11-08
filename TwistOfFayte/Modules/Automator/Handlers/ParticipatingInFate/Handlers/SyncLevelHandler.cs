using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;
using FateState = Dalamud.Game.ClientState.Fates.FateState;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class SyncLevelHandler(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    CombatConfig combat,
    IPlayer player
) : BaseHandler(ParticipatingInFateState.SyncLevel, state, fates, npcs, combat)
{
    public override StatePriority GetScore()
    {
        var fate = GetFate();
        if (fate == null || fate.State == FateState.Preparation)
        {
            return StatePriority.Never;
        }

        unsafe
        {
            if (PlayerState.Instance()->IsLevelSynced)
            {
                return StatePriority.Never;
            }
        }

        return fate.MaxLevel < player.GetLevel() ? StatePriority.Always : StatePriority.Never;
    }

    public override void Handle()
    {
        if (!EzThrottler.Throttle("SyncLevel"))
        {
            return;
        }

        unsafe
        {
            FateManager.Instance()->LevelSync();
        }
    }
}
