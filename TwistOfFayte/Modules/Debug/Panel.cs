namespace TwistOfFayte.Modules.Debug;

public class Panel
{
    public void Render(RenderContext context)
    {
        OcelotUI.Title("Debug:");
        OcelotUI.Indent(() => {
            // Content here
        });
    }
}
