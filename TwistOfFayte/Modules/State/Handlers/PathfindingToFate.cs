using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using ECommons.GameHelpers;
using ECommons.Throttlers;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.PathfindingToFate)]
public class PathfindingToFate(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    private Vector3? Destination = null;

    private bool IsPathingToNpc = false;

    private bool IsComplete = false;

    public override void Enter()
    {
        Destination = null;
        IsPathingToNpc = false;
        IsComplete = false;

        Destination = FateHelper.SelectedFate?.GetDestination();
    }

    public override State? Handle()
    {
        if (FateHelper.SelectedFate == null || Destination == null)
        {
            Module.Debug("Fate ended, returning to idle");
            return State.Idle;
        }

        if (IsComplete && FateHelper.CurrentFate?.IsSelected() == true)
        {
            return FateHelper.SelectedFate.State == FateState.Preparation ? State.StartingFate : State.ParticipatingInFate;
        }

        // Path to starting NPC if needed
        if (FateHelper.SelectedFate.IsInPreparation() && !IsPathingToNpc && TargetHelper.Friendlies.Any())
        {
            var npc = TargetHelper.Friendlies.First();
            IsPathingToNpc = true;

            Prowler.Prowl(new Prowl(npc) {
                ShouldMount = prowl => prowl.PathLength >= 30f,
                PreProcessor = prowl => {
                    if (prowl.GameObject == null)
                    {
                        return;
                    }

                    prowl.Destination = prowl.OriginalDestination.GetPointFromPlayer(prowl.GameObject.HitboxRadius, prowl.GameObject.HitboxRadius + 2f);
                },
                PostProcessor = prowl => prowl.Nodes = prowl.Nodes.ContinueFrom(Player.Position).Smooth(),
                Watcher = prowl => Player.DistanceTo(prowl.Destination) <= 5f,
                OnComplete = (_, _) => IsComplete = true,
            });
        }

        var distance = Player.DistanceTo(Destination!.Value);
        if (distance <= 5f) // @todo: make configurable?
        {
            Module.Debug($"Close enough to Destination {Destination:f2} ({distance}/5.0)");
            Module.VNavmesh.Stop();
            return FateHelper.SelectedFate.State == FateState.Preparation ? State.StartingFate : State.ParticipatingInFate;
        }

        if (Destination.HasValue && !Prowler.IsRunning)
        {
            Prowler.Prowl(new Prowl(Destination.Value) {
                ShouldFly = prowl => prowl.EuclideanDistance >= 30f,
                ShouldMount = prowl => prowl.PathLength >= 30f,
                PostProcessor = prowl => prowl.Nodes = prowl.Nodes.Smooth(),
                Watcher = prowl => Player.DistanceTo(prowl.Destination) <= 5f,
                OnComplete = (_, _) => IsComplete = true,
            });
        }

        if (!Player.Mounted && !Player.Mounting && EzThrottler.Throttle("Mount", 2500))
        {
            Module.Debug("Ensuring player is mounted");
            Plugin.Chain.Submit(Actions.MountRoulette.GetCastChain());
        }

        return null;
    }
}
