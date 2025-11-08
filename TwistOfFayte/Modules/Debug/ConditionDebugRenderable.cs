using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.UI.Services;

namespace TwistOfFayte.Modules.Debug;

public class ConditionDebugRenderable(
    ICondition condition,
    IUIService ui,
    IBrandingService branding
) : IDebugRenderable
{
    public void Render()
    {
        ui.Text("Player Conditions:", branding.DalamudYellow);
        ImGui.Indent();
        foreach (var flag in Enum.GetValues<ConditionFlag>())
        {
            if (condition[flag])
            {
                ui.LabelledValue(flag.ToString(), "true");
            }
        }
        ImGui.Unindent();
    }
}
