using Ocelot.Lifecycle;
using Ocelot.Services.Translation;

namespace TwistOfFayte.Modules.Translations;

public class TranslationLoader(ITranslationRepository translations) : IOnStart
{
    public void OnStart()
    {
        translations.LoadFromDirectory("Translations", "en");
    }
}
