using System;
using Ocelot.States;
using Ocelot.States.Flow;
using Ocelot.UI.Services;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate;

public class Handler(Func<IStateMachine<TravellingToFateState>> factory, IStateManager state, IUIService ui)
    : FlowStateHandler<AutomatorState>(AutomatorState.TravellingToFate)
{
    private IStateMachine<TravellingToFateState>? stateMachine;

    public override void Enter()
    {
        base.Enter();
        stateMachine ??= factory();
    }

    public override void Exit(AutomatorState next)
    {
        base.Exit(next);
        stateMachine = null;
    }

    public override AutomatorState? Handle()
    {
        if (!state.IsActive())
        {
            return AutomatorState.WaitingForFate;
        }

        stateMachine?.Update();

        if (stateMachine?.State == TravellingToFateState.Arrived)
        {
            return AutomatorState.ParticipatingInFate;
        }

        return null;
    }

    public override void Render()
    {
        ui.LabelledValue("Substate", stateMachine?.State.ToString() ?? "Unknown");
        stateMachine?.StateHandler.Render();
    }
}
