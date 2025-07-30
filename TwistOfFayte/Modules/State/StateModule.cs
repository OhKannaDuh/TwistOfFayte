namespace TwistOfFayte.Modules.State;

[OcelotModule]
public class StateModule : Module
{
    public bool IsRunning = false;

    public override bool ShouldUpdate {
        get => IsRunning;
    }

    public readonly StateMachine<State, StateModule> StateMachine;

    public StateModule(Plugin plugin, Config config)
        : base(plugin, config)
    {
        StateMachine = new StateMachine<State, StateModule>(State.Idle, this);
    }

    public override void Update(UpdateContext context)
    {
        StateMachine.Update();
    }
}
