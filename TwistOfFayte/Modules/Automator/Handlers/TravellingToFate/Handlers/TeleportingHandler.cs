using System.Numerics;
using ECommons.Throttlers;
using Ocelot.Services.PlayerState;
using Ocelot.States.Flow;
using Ocelot.UI.Services;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class TeleportingHandler(TravellingToFateContext context, IPlayer player, IUIService ui) : FlowStateHandler<TravellingToFateState>(TravellingToFateState.Teleporting)
{
    private enum SubState
    {
        WaitingToCast,
        WaitingToBeBetweenAreas,
        WaitingToBeDone,
    }
    
    private SubState subState = SubState.WaitingToCast;

    public override void Enter()
    {
        base.Enter();
        subState = SubState.WaitingToCast;
    }

    public override TravellingToFateState? Handle()
    {
        if (context.chosenPath == null)
        {
            return TravellingToFateState.Arrived;
        }

        if (context.chosenAetheryte == null)
        {
            return TravellingToFateState.Mounting;
        }

        if (subState == SubState.WaitingToCast && EzThrottler.Throttle("Teleport Cast"))
        {
            context.chosenAetheryte.Teleport();
            subState = SubState.WaitingToBeBetweenAreas;
        }

        if (subState == SubState.WaitingToBeBetweenAreas && player.IsBetweenAreas())
        {
            subState = SubState.WaitingToBeDone;
        }

        var distance = Vector3.Distance(player.GetPosition(), context.chosenAetheryte.Position);
        if (distance <= 30f && subState == SubState.WaitingToBeDone)
        {
            return TravellingToFateState.Mounting;
        }

        return null;
    }
    
    

    public override void Render()
    {
        ui.LabelledValue("Teleporting State", subState);
    }
}
