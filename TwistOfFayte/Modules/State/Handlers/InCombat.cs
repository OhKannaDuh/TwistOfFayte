using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;

namespace TwistOfFayte.Modules.State.Handlers;

[State<State>(State.InCombat)]
public class InCombat(StateModule module, StateMachine<State, StateModule> stateMachine) : StateHandler<State, StateModule>(module, stateMachine)
{
    public override State? Handle()
    {
        if (!Svc.Condition[ConditionFlag.InCombat])
        {
            return State.Idle;
        }

        if (FateHelper.IsInSelectedFate())
        {
            return State.ParticipatingInFate;
        }

        Svc.Targets.Target ??= Svc.Objects.OfType<IBattleNpc>()
            .Where(o => o is {
                IsDead: false,
                IsTargetable: true,
            })
            .Where(o => o.IsHostile() && o.IsTargetingPlayer())
            .OrderBy(Player.DistanceTo)
            .FirstOrDefault();

        return null;
    }
}
