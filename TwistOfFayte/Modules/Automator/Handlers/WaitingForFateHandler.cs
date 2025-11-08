using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.Services.Logger;
using Ocelot.States.Flow;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Materia;
using TwistOfFayte.Services.Repair;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class WaitingForFateHandler(
    IStateManager state,
    ILogger logger,
    ICondition condition,
    IRepairService repair,
    IMateriaExtractionService  materiaExtraction,
    MultiZoneConfig multiZoneConfig
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

        if (repair.ShouldRepair())
        {
            return AutomatorState.Repairing;
        }

        if (materiaExtraction.ShouldExtract())
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
            logger.Info("Transitioning to Participating in Fate ");
            return AutomatorState.ParticipatingInFate;
        }

        logger.Info("Transitioning to Travelling to Fate {}");
        return AutomatorState.TravellingToFate;
    }
}
