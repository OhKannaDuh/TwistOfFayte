using ECommons.GameHelpers;

namespace TwistOfFayte.Modules.State.Handlers.FateAi;

[State<FateAiState>(FateAiState.MaintainFateZone)]
public class MaintainFateZone(StateModule module, FateAiStateMachine stateMachine) : Handler(module, stateMachine)
{
    private bool isComplete = false;

    public override void Enter()
    {
        isComplete = false;
        Prowler.Abort();

        if (FateHelper.CurrentFate == null)
        {
            isComplete = true;
            return;
        }

        Prowler.Prowl(new Prowl(FateHelper.CurrentFate.Position.GetPointFromPlayer(5f, 2f)) {
            ShouldFly = _ => false,
            ShouldMount = _ => false,
            PostProcessor = prowl => prowl.Nodes = prowl.Nodes.Smooth(),
            Watcher = prowl => Player.DistanceTo(prowl.Destination) <= 1f,
            OnComplete = (_, _) => isComplete = true,
            OnCancel = (_, _) => isComplete = true,
        });
    }

    public override FateAiState? Handle()
    {
        return FateHelper.CurrentFate == null || isComplete || !Prowler.IsRunning ? StateMachine.MakeChoice() : null;
    }
}
