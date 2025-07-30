namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.ChangingZone)]
public class ChangingZone(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override State? Handle()
    {
        return null;
    }
}
