using System;
using Ocelot.Services.Translation;
using Ocelot.Windows;
using TwistOfFayte.Renderers.Help;

namespace TwistOfFayte.Windows;

public class HelpWindow : OcelotWindow, IDisposable
{
    private readonly IHelpRenderer renderer;

    private readonly ITranslator translator;

    public HelpWindow(IHelpRenderer renderer, ITranslator<HelpWindow> translator) : base(translator.T(".title"))
    {
        this.renderer = renderer;
        this.translator = translator;

        translator.LanguageChanged += UpdateWindowTitle;
        translator.TranslationsChanged += UpdateWindowTitle;
    }

    protected override void Render()
    {
        renderer.Render();
    }

    private void UpdateWindowTitle()
    {
        WindowName = translator.T(".title");
    }

    public void Dispose()
    {
        translator.TranslationsChanged -= UpdateWindowTitle;
        translator.LanguageChanged -= UpdateWindowTitle;
    }
}
