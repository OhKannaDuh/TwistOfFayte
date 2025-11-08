using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.Chain;
using Ocelot.States.Flow;
using TwistOfFayte.Services.Repair;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class RepairingHandler(
    IStateManager state,
    ICondition condition,
    IRepairService repair
) : FlowStateHandler<AutomatorState>(AutomatorState.Repairing)
{
    private Task<ChainResult>? task;

    private CancellationTokenSource cancel = new();

    public override void Enter()
    {
        base.Enter();

        task = null;
        cancel = new CancellationTokenSource();
    }

    public override AutomatorState? Handle()
    {
        if (!state.IsActive())
        {
            return null;
        }

        if (condition[ConditionFlag.InCombat] || !repair.ShouldRepair())
        {
            cancel.Cancel();
            return AutomatorState.WaitingForFate;
        }

        task ??= repair.Repair().ExecuteAsync(cancel.Token);

        return task?.IsCompleted == true ? AutomatorState.WaitingForFate : null;
    }
}
