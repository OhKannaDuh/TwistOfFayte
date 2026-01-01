using Dalamud.Bindings.ImGui;
using Ocelot.Services.UI;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Debug;

public class NpcsDebugRenderable(
    INpcProvider npcs,
    IUIService ui,
    IBrandingService branding
) : IDebugRenderable
{
    public void Render()
    {
        ui.Text("Npcs:", branding.DalamudYellow);
        ImGui.Indent();
        foreach (var npc in npcs.GetEnemies())
        {
            ui.Text(npc.Position);
        }

        ImGui.Unindent();
    }
}
