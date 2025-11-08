using Ocelot.States.Flow;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class ArrivedHandler() : FlowStateHandler<TravellingToFateState>(TravellingToFateState.Arrived)
{
    public override TravellingToFateState? Handle()
    {
        return null;
    }
}
