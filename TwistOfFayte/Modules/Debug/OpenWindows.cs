using Ocelot.Lifecycle;
using Ocelot.Windows;
using TwistOfFayte.Windows;

namespace TwistOfFayte.Modules.Debug;

public class OpenWindows(IMainWindow? window = null, IConfigWindow? config = null, HelpWindow? help = null) : IOnStart
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
        
        if (help != null)
        {
            help.IsOpen = true;
        }
    }
}
