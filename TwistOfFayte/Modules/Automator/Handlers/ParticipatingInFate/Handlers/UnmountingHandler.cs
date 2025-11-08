using ECommons.Throttlers;
using Ocelot.Actions;
using Ocelot.Services.PlayerState;
using Ocelot.States.Score;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class UnmountingHandler(IPlayer player) : ScoreStateHandler<ParticipatingInFateState, StatePriority>(ParticipatingInFateState.Unmounting)
{
    public override StatePriority GetScore()
    {
        return player.IsMounted() || player.IsMounting() ? StatePriority.Always : StatePriority.Never;
    }

    public override void Handle()
    {
        if (!EzThrottler.Throttle("Unmount"))
        {
            return;
        }

        Actions.Unmount.Cast();
    }
}
