using Ocelot.Services.Pathfinding;
using Ocelot.States.Score;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate.Handlers;

public class EntranceHandler(IPathfinder pathfinder) : ScoreStateHandler<ParticipatingInFateState, StatePriority>(ParticipatingInFateState.Entrance)
{
    public override StatePriority GetScore()
    {
        return StatePriority.Never;
    }

    public override void Handle()
    {
        pathfinder.Stop();
    }
}
