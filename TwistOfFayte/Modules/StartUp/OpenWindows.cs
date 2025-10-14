using System;
using Ocelot.Lifecycle;
using Ocelot.Services.Translation;
using Ocelot.Windows;

namespace TwistOfFayte.Modules.StartUp;

public class OpenWindows(ITranslationRepository translations, IMainWindow? window = null, IConfigWindow? config = null) : IOnStart, IOnLoad
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

    public void OnLoad()
    {
        translations.LoadFromDirectory("Translations/en", "en");
        throw new Exception("Tset");
    }
}
