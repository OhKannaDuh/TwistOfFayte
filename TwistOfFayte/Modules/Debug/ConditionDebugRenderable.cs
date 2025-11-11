using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Ocelot.Services.UI;

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
                ui.Text(flag.ToString(), branding.DalamudYellow);
            }
        }

        ImGui.Unindent();
    }
}
