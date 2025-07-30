namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.ChangingInstance)]
public class ChangingInstance(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override State? Handle()
    {
        return null;
    }
}
