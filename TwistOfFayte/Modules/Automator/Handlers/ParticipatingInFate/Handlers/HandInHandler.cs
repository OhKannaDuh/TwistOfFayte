using Ocelot.States.Score;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class HandInHandler() : ScoreStateHandler<ParticipatingInFateState, StatePriority>(ParticipatingInFateState.HandIn)
{
    public override StatePriority GetScore()
    {
        return StatePriority.Never;
    }

    public override void Handle()
    {
    }
}
