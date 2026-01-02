using System;
using System.Numerics;
using ECommons.Throttlers;
using Ocelot.Services.PlayerState;
using Ocelot.Services.UI;
using Ocelot.States.Flow;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class TeleportingHandler(TravellingToFateContext context, IPlayer player, IUIService ui)
    : FlowStateHandler<TravellingToFateState>(TravellingToFateState.Teleporting)
{
    private enum SubState
    {
        WaitingToCast,
        WaitingToBeBetweenAreas,
        WaitingToBeDone,
    }

    private SubState subState = SubState.WaitingToCast;

    private DateTime subStateEnteredAt;

    private double TimeInSubStateSeconds
    {
        get => (DateTime.UtcNow - subStateEnteredAt).TotalSeconds;
    }

    public override void Enter()
    {
        base.Enter();
        ChangeState(SubState.WaitingToCast);
    }

    private void ChangeState(SubState state)
    {
        if (state == subState)
        {
            return;
        }

        subState = state;
        subStateEnteredAt = DateTime.Now;
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

        // Already between areas - skip to waiting for completion
        if (player.IsBetweenAreas())
        {
            ChangeState(SubState.WaitingToBeDone);
            return null;
        }

        // Already casting - wait for it to complete
        if (player.IsCasting())
        {
            if (subState == SubState.WaitingToCast)
            {
                ChangeState(SubState.WaitingToBeBetweenAreas);
            }

            return null;
        }

        if (subState == SubState.WaitingToCast && EzThrottler.Throttle("Teleport Cast"))
        {
            context.chosenAetheryte.Teleport();
            ChangeState(SubState.WaitingToBeBetweenAreas);
        }

        if (subState == SubState.WaitingToBeBetweenAreas && player.IsBetweenAreas())
        {
            ChangeState(SubState.WaitingToBeDone);
        }

        var distance = Vector3.Distance(player.GetPosition(), context.chosenAetheryte.Position);
        if (distance <= 30f && subState == SubState.WaitingToBeDone)
        {
            return TravellingToFateState.Mounting;
        }

        if (TimeInSubStateSeconds >= 10)
        {
            ChangeState(SubState.WaitingToCast);
        }

        return null;
    }


    public override void Render()
    {
        ui.LabelledValue("Teleporting State", subState);
    }
}
