using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.Chain;
using Ocelot.States.Flow;
using TwistOfFayte.Services.Materia;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class ExtractingMateriaHandler(
    IStateManager state,
    ICondition condition,
    IMateriaExtractionService materiaExtraction
) : FlowStateHandler<AutomatorState>(AutomatorState.ExtractingMateria)
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

        if (condition[ConditionFlag.InCombat] || !materiaExtraction.ShouldExtract())
        {
            cancel.Cancel();
            return AutomatorState.WaitingForFate;
        }

        task ??= materiaExtraction.ExtractEquipped().ExecuteAsync(cancel.Token);

        return task?.IsCompleted == true ? AutomatorState.WaitingForFate : null;
    }
}
