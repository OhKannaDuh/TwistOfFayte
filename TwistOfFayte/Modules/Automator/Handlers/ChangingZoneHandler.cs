using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Ocelot.Services.ClientState;
using Ocelot.Services.PlayerState;
using Ocelot.Services.UI;
using Ocelot.States.Flow;
using TwistOfFayte.Config;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Modules.Automator.Handlers;

public class ChangingZoneHandler(
    IStateManager state,
    ICondition condition,
    IPlayer player,
    IClient client,
    MultiZoneConfig multiZoneConfig,
    IUIService ui
) : FlowStateHandler<AutomatorState>(AutomatorState.ChangingZone)
{
    private enum SubState
    {
        WaitingToCast,
        WaitingToBeBetweenAreas,
        WaitingToBeDone,
    }

    private SubState subState = SubState.WaitingToCast;

    private uint zone = 0;

    private uint aetheryte = 0;

    public override void Enter()
    {
        base.Enter();

        subState = SubState.WaitingToCast;
        zone = multiZoneConfig.GetNextZone(client);
        aetheryte = GetAetheryteId(zone);
    }

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

        if (aetheryte == 0)
        {
            return AutomatorState.WaitingForFate;
        }

        // Already between areas - skip to waiting for completion
        if (player.IsBetweenAreas())
        {
            if (subState != SubState.WaitingToBeDone)
            {
                subState = SubState.WaitingToBeDone;
            }
            return null;
        }

        // Already casting - wait for it to complete
        if (player.IsCasting())
        {
            if (subState == SubState.WaitingToCast)
            {
                subState = SubState.WaitingToBeBetweenAreas;
            }
            return null;
        }

        if (subState == SubState.WaitingToCast && EzThrottler.Throttle("Teleport Cast"))
        {
            unsafe
            {
                Telepo.Instance()->Teleport(aetheryte, 0);
            }

            subState = SubState.WaitingToBeBetweenAreas;
        }

        if (subState == SubState.WaitingToBeBetweenAreas && player.IsBetweenAreas())
        {
            subState = SubState.WaitingToBeDone;
        }

        if (client.Player != null && subState == SubState.WaitingToBeDone && client.CurrentTerritoryId == zone)
        {
            return AutomatorState.WaitingForFate;
        }

        return null;
    }

    private uint GetAetheryteId(uint zone)
    {
        unsafe
        {
            var teleport = Telepo.Instance()->TeleportList.FirstOrNull(t => t.TerritoryId == zone);

            return teleport?.AetheryteId ?? 0;
        }
    }

    public override void Render()
    {
        ui.LabelledValue("Zone change state", subState);
    }
}
