using System;
using Ocelot.Services.UI;
using Ocelot.States;
using Ocelot.States.Flow;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate;

public class Handler(
    Func<IStateMachine<TravellingToFateState>> factory,
    IStateManager state,
    IFateSelector selector,
    IUIService ui
)
    : FlowStateHandler<AutomatorState>(AutomatorState.TravellingToFate)
{
    private IStateMachine<TravellingToFateState>? stateMachine;

    private bool changed = false;

    public override void Enter()
    {
        base.Enter();
        selector.SelectionChanged += SelectionChanged;
        stateMachine ??= factory();
        changed = false;
    }

    public override void Exit(AutomatorState next)
    {
        base.Exit(next);
        selector.SelectionChanged -= SelectionChanged;
        stateMachine?.StateHandler.Exit(TravellingToFateState.Arrived);
        stateMachine = null;
        changed = false;
    }

    public override AutomatorState? Handle()
    {
        if (!state.IsActive() || changed)
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

    private void SelectionChanged(FateId _)
    {
        changed = true;
    }
}
