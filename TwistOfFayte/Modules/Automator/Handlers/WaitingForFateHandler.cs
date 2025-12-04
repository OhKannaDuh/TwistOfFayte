using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.States.Flow;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Materia;
using TwistOfFayte.Services.Repair;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class WaitingForFateHandler(
    IStateManager state,
    ICondition condition,
    IRepairService repair,
    IMateriaExtractionService materiaExtraction,
    MultiZoneConfig multiZoneConfig,
    GeneralConfig generalConfig
) : FlowStateHandler<AutomatorState>(AutomatorState.WaitingForFate)
{
    public override AutomatorState? Handle()
    {
        if (!state.IsActive())
        {
            return null;
        }

        if (condition[ConditionFlag.InCombat])
        {
            return AutomatorState.InCombat;
        }

        if (generalConfig.ShouldAutoRepair && repair.ShouldRepair())
        {
            return AutomatorState.Repairing;
        }

        if (generalConfig.ShouldAutoExtractMateria && materiaExtraction.ShouldExtract())
        {
            return AutomatorState.ExtractingMateria;
        }

        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            if (multiZoneConfig.Zones.Count > 0 && TimeInState > TimeSpan.FromSeconds(multiZoneConfig.WaitForFateDelay))
            {
                return AutomatorState.ChangingZone;
            }

            return null;
        }

        var current = state.GetCurrentFate();
        if (current != null && selected == current)
        {
            return AutomatorState.ParticipatingInFate;
        }

        return AutomatorState.TravellingToFate;
    }
}
