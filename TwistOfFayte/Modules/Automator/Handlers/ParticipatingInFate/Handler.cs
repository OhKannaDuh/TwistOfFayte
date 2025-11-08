using System;
using Ocelot.States;
using Ocelot.States.Flow;
using Ocelot.UI.Services;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate;

public class Handler(
    Func<IStateMachine<ParticipatingInFateState>> factory,
    IStateManager state,
    IUIService ui
) : FlowStateHandler<AutomatorState>(AutomatorState.ParticipatingInFate)
{
    private IStateMachine<ParticipatingInFateState>? stateMachine;

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

        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            return AutomatorState.WaitingForFate;
        }

        stateMachine?.Update();

        return null;
    }

    public override void Render()
    {
        ui.LabelledValue("Substate", stateMachine?.State.ToString() ?? "Unknown");
        stateMachine?.StateHandler.Render();
    }
}
