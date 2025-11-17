using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Ocelot.Services.Translation;
using Ocelot.Services.UI;

namespace TwistOfFayte.Renderers.Help;

public class HelpRenderer(
    ITranslator<HelpRenderer> translator,
    IUIService ui,
    IBrandingService branding
) : IHelpRenderer
{
    private readonly List<string> GeneralTipKeys =
    [
        "blacklist",
        "byop",
        "ranged_mobs",
        "issues"
    ];

    public void Render()
    {
        ui.Text(translator.T(".general_tips.title"), branding.DalamudYellow);
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0f, 4f));

        ImGui.Indent(16);
        foreach (var generalTipKey in GeneralTipKeys)
        {
            ImGui.TextWrapped($"-  {translator.T($".general_tips.tips.{generalTipKey}")}");
        }
        ImGui.Unindent();
    }
}
