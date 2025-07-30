using ECommons.DalamudServices;
using ECommons.Throttlers;
using TwistOfFayte.Modules.General;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.ExtractMateria)]
public class ExtractMateria(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override State? Handle()
    {
        if (Plugin.Chain.IsRunning)
        {
            return null;
        }

        if (!EzThrottler.Throttle("ExtractMateriaState.Check", 100))
        {
            return null;
        }

        if (MateriaHelper.CanExtract() && !Plugin.Chain.IsRunning)
        {
            MateriaHelper.ExtractEquipped();
            return null;
        }

        Svc.Log.Info("Chain is running: " + (Plugin.Chain.IsRunning ? "Yes" : "No"));

        return State.Idle;
    }
}
