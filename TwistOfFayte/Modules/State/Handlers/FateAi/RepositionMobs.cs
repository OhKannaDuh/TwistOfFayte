using System.Linq;
using System.Numerics;
using ECommons.GameHelpers;
using Ocelot.Extensions;

namespace TwistOfFayte.Modules.State.Handlers.FateAi;

[State<FateAiState>(FateAiState.RepositionMobs)]
public class RepositionMobs(StateModule module, FateAiStateMachine stateMachine) : Handler(module, stateMachine)
{
    private bool isComplete = false;

    public override void Enter()
    {
        isComplete = false;

        var mobs = TargetHelper.InCombat.ToList();
        if (mobs.Count < 2)
        {
            module.Debug("[RepositionMobs] Less than two mobs in combat. Skipping reposition.");
            isComplete = true;
            return;
        }

        if (FateHelper.CurrentFate == null)
        {
            module.Debug("[RepositionMobs] No longer in fate.");
            isComplete = true;
            return;
        }


        var radius = mobs.Max(Player.DistanceTo);
        var direction = Vector3.Normalize(FateHelper.CurrentFate.Position - Player.Position);
        var destination = Player.Position + direction * radius;


        if (FateHelper.CurrentFate.Position.Distance2D(destination) > FateHelper.CurrentFate.Radius - 2f)
        {
            destination = FateHelper.CurrentFate.Position +
                          Vector3.Normalize(destination - FateHelper.CurrentFate.Position) * (FateHelper.CurrentFate.Radius - 2f);
            module.Debug("[RepositionMobs] Adjusted destination to stay within FATE radius.");
        }

        module.Debug($"[RepositionMobs] Moving to point {destination} (max mob distance: {radius:f2})");

        Prowler.Prowl(new Prowl(destination) {
            ShouldFly = _ => false,
            ShouldMount = _ => false,
            PostProcessor = prowl => prowl.Nodes = prowl.Nodes.Smooth(),
            Watcher = prowl => Player.DistanceTo(prowl.Destination) <= 0.5f,
            OnCancel = (_, _) => isComplete = true,
            OnComplete = (_, _) => isComplete = true,
        });
    }

    public override FateAiState? Handle()
    {
        return isComplete ? StateMachine.MakeChoice() : null;
    }
}
