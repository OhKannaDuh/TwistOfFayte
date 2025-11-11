using System.Collections.Generic;
using Ocelot.Graphics;
using Ocelot.Lifecycle;
using Ocelot.Services.OverlayRenderer;
using TwistOfFayte.Modules.Debug;
using TwistOfFayte.Services.Zone;

namespace TwistOfFayte.Renderers;

public class DebugRenderer(IEnumerable<IDebugRenderable> renderables, IZone zone, IOverlayRenderer overlay) : IOnRender
{
    public void RenderPanel()
    {
        foreach (var renderable in renderables)
        {
            renderable.Render();
        }
    }

    public void Render()
    {
#if DEBUG
        foreach (var aetheryte in zone.Aetherytes)
        {
            overlay.StrokeCircle(aetheryte.Position, 5f, Color.Red);
        }
#endif
    }
}
