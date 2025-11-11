using System.Numerics;
using Dalamud.Bindings.ImGui;
using Ocelot.Services.WindowManager;
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
        if (fateListRenderer.ShouldRender())
        {
            fateListRenderer.Render();

            ImGui.Dummy(new Vector2(0.0f, 16.0f));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0.0f, 16.0f));
        }

        automationStateRenderer.Render();

        if (debugConfig.ShowDebug)
        {
            ImGui.Dummy(new Vector2(0.0f, 16.0f));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0.0f, 16.0f));
            debugRenderer.RenderPanel();
        }
    }
}
