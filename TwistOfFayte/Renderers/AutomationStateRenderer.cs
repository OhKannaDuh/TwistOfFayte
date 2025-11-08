using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using Ocelot.States;
using Ocelot.UI.Services;
using TwistOfFayte.Modules.Automator;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Renderers;

public class AutomationStateRenderer(
    IStateMachine<AutomatorState> stateMachine,
    IStateManager state,
    IPlayer player,
    ITargetManager target,
    IUIService ui
)
{
    public void Render()
    {
        ui.LabelledValue("Is Active", state.IsActive());
        if (state.IsActive())
        {
            ui.LabelledValue("State", stateMachine.State);
            stateMachine.StateHandler.Render();
        }

        if (ImGui.Button("Toggle Automation"))
        {
            state.Toggle();
        }

        if (target.Target != null)
        {
            unsafe
            {
                var chara = (BattleChara*)target.Target.Address;
                if (chara != null)
                {
                    ui.LabelledValue("Target Name Id", chara->NameId);
                    ui.LabelledValue("Target Distance", player.GetPosition().Distance2D(target.Target.Position));
                    ui.LabelledValue("Target Hitbox", target.Target.HitboxRadius);

                    var startC = chara->DefaultPosition;
                    var start = new Vector3(startC.X, startC.Y, startC.Z);

                    var posC = chara->Position;
                    var pos = new Vector3(posC.X, posC.Y, posC.Z);

                    var distance = start.Distance(pos);

                    ui.LabelledValue("Distance from start", distance.ToString("f2"));
                }
            }
        }
    }
}
