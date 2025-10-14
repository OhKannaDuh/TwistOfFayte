using Ocelot.Services.WindowManager;

namespace TwistOfFayte.Renderers;

public class RenderComposer(FateListRenderer fateListRenderer) : IMainRenderer
{
    public void Render()
    {
        fateListRenderer.Render();
    }
}
