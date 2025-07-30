namespace TwistOfFayte.Modules.State.Handlers.FateAi;

public abstract class Handler(StateModule module, FateAiStateMachine stateMachine) : StateHandler<FateAiState, StateModule>(module, stateMachine)
{
    protected override FateAiStateMachine StateMachine {
        get => stateMachine;
    }
}
