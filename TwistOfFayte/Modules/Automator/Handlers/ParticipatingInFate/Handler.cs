using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Ocelot.Services.UI;
using Ocelot.States;
using Ocelot.States.Flow;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.State;
using FateState = Dalamud.Game.ClientState.Fates.FateState;

namespace TwistOfFayte.Modules.Automator.Handlers.ParticipatingInFate;

public class Handler(
    Func<IStateMachine<ParticipatingInFateState>> factory,
    IStateManager state,
    IFateRepository fates,
    IFateSelector selector,
    IUIService ui
) : FlowStateHandler<AutomatorState>(AutomatorState.ParticipatingInFate)
{
    private IStateMachine<ParticipatingInFateState>? stateMachine;

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
        stateMachine?.StateHandler.Exit(ParticipatingInFateState.Entrance);
        stateMachine = null;
        changed = false;
    }

    public override AutomatorState? Handle()
    {
        if (!state.IsActive() || changed)
        {
            return AutomatorState.WaitingForFate;
        }

        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            return AutomatorState.WaitingForFate;
        }

        var snapshot = fates.Snapshot();
        var fate = snapshot.FirstOrDefault(f => f.Id == selected.Value);
        if (fate == null)
        {
            return AutomatorState.WaitingForFate;
        }

        unsafe
        {
            if (fate.State != FateState.Preparation && FateManager.Instance()->CurrentFate == null)
            {
                return AutomatorState.WaitingForFate;
            }
        }

        stateMachine?.Update();

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
