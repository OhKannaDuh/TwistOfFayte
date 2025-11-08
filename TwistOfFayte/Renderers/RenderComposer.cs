using Dalamud.Bindings.ImGui;
using Ocelot.Services.WindowManager;
using Ocelot.UI.Services;
using TwistOfFayte.Config;

namespace TwistOfFayte.Renderers;

public class RenderComposer(
    FateListRenderer fateListRenderer,
    AutomationStateRenderer automationStateRenderer,
    DebugRenderer debugRenderer,
    DebugConfig debugConfig
) : IMainRenderer
{
    public void Render()
    {
        fateListRenderer.Render();
        ImGui.Separator();
        automationStateRenderer.Render();

        if (debugConfig.ShowDebug)
        {
            ImGui.Separator();
            debugRenderer.RenderPanel();
        }
    }
}
