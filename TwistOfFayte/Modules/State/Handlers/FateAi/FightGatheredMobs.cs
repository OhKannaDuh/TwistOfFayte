using System;
using System.Linq;
using System.Numerics;
using ECommons.DalamudServices;
using ECommons.GameHelpers;

namespace TwistOfFayte.Modules.State.Handlers.FateAi;

[State<FateAiState>(FateAiState.FightGatheredMobs)]
public class FightGatheredMobs(StateModule module, FateAiStateMachine stateMachine) : Handler(module, stateMachine)
{
    private bool isComplete = false;

    private DateTime EnterTime = DateTime.Now;

    private bool IsComplete {
        get => (isComplete || !TargetHelper.InCombat.Any() || !Prowler.IsRunning) && DateTime.Now.Subtract(EnterTime).TotalSeconds > 1;
    }

    public override void Enter()
    {
        isComplete = false;
        EnterTime = DateTime.Now;
        Prowler.Abort();


        var destination = Vector3.Zero;
        if (TargetHelper.InCombat.Count() == 1)
        {
            var mob = TargetHelper.InCombat.First();

            var range = Player.Job.GetRange();
            var distance = Player.DistanceTo(mob);
            if (distance > mob.HitboxRadius && distance < range + mob.HitboxRadius)
            {
                Svc.Targets.Target ??= mob;
                Plugin.Chain.Submit(chain => chain.Wait(1000).Then(_ => isComplete = true));
                return;
            }


            destination = mob.Position.GetPointFromPlayer(mob.HitboxRadius + 2, mob.HitboxRadius);
        }
        else
        {
            foreach (var mob in TargetHelper.InCombat)
            {
                destination += mob.Position;
            }

            destination /= TargetHelper.InCombat.Count();
        }

        Prowler.Prowl(new Prowl(destination.GetPointFromPlayer(0.5f)) {
            ShouldFly = _ => false,
            ShouldMount = _ => false,
            PostProcessor = prowl => prowl.Nodes = prowl.Nodes.Smooth(),
            Watcher = _ => {
                Svc.Targets.Target ??= TargetHelper.InCombat.First();
                return false;
            },
            OnComplete = (_, _) => isComplete = true,
            OnCancel = (_, _) => isComplete = true,
        });
    }

    public override FateAiState? Handle()
    {
        return IsComplete ? StateMachine.MakeChoice() : null;
    }
}
