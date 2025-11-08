using Ocelot.Lifecycle;
using Ocelot.States;

namespace TwistOfFayte.Modules.Automator;

public class AutomatorModule(IStateMachine<AutomatorState> stateMachine) : IOnUpdate
{
    public void Update()
    {
        stateMachine.Update();
    }
}
