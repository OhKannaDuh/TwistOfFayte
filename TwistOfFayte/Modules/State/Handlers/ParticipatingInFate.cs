using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using TwistOfFayte.Modules.State.Handlers.FateAi;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.ParticipatingInFate)]
public class ParticipatingInFate(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    private IGameObject? lastTarget = null;

    public readonly FateAiStateMachine StateMachine = new(module);

    public override void Enter()
    {
        Svc.Targets.Target = null;
        Module.TargetModule.ShouldTarget = true;
        TargetHelper.OnTargetChanged += OnTargetChanged;
        StateMachine.Reset();
    }

    public override void Exit()
    {
        Module.TargetModule.ShouldTarget = false;
        TargetHelper.OnTargetChanged -= OnTargetChanged;
        Prowler.Abort();
    }

    private void OnTargetChanged(IGameObject? target)
    {
        lastTarget = target;
        Prowler.Abort();
    }

    public override unsafe State? Handle()
    {
        if (Player.Mounted)
        {
            Actions.TryUnmount();
        }

        if (!FateHelper.IsInFate())
        {
            return State.Idle;
        }

        if (FateHelper.CurrentFate?.NeedSync() == true && EzThrottler.Throttle("Fate Sync"))
        {
            FateManager.Instance()->LevelSync();
        }

        StateMachine.Update();

        return null;
    }
}
