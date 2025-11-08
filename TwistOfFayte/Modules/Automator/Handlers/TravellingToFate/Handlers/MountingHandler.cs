using ECommons.Throttlers;
using Ocelot.Actions;
using Ocelot.Services.PlayerState;
using Ocelot.States.Flow;
using TwistOfFayte.Config;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class MountingHandler(IPlayer player, TraversalConfig config) : FlowStateHandler<TravellingToFateState>(TravellingToFateState.Mounting)
{
    public override TravellingToFateState? Handle()
    {
        if (!player.IsMounted())
        {
            if (!player.IsMounting() && EzThrottler.Throttle("Mounting"))
            {
                if (config.UseMountRoulette)
                {
                    Actions.MountRoulette.Cast();
                }
                else
                {
                    Actions.Mount(config.MountId).Cast();
                }
            }

            return null;
        }

        return TravellingToFateState.Traversing;
    }
}
