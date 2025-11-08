using Ocelot.Lifecycle;
using Ocelot.Windows;

namespace TwistOfFayte.Modules.Debug;

public class OpenWindows(IMainWindow? window = null, IConfigWindow? config = null) : IOnStart
{
    public void OnStart()
    {
        if (window != null)
        {
            window.IsOpen = true;
        }

        if (config != null)
        {
            config.IsOpen = true;
        }
    }
}
