using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using TwistOfFayte.Modules.General;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.Idle)]
public class Idle(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override State? Handle()
    {
        if (Svc.Condition[ConditionFlag.InCombat])
        {
            return State.InCombat;
        }

        if (MateriaHelper.CanExtract())
        {
            return State.ExtractMateria;
        }

        if (FateHelper.SelectedFate != null)
        {
            return State.PathfindingToFate;
        }

        return null;
    }
}
