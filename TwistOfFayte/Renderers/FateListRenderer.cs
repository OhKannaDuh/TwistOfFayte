using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Fates;
using Ocelot.UI.ComposableStrings;
using Ocelot.UI.Services;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Renderers;

public class FateListRenderer(
    IFateRepository fates,
    IStateManager state,
    IFateScorer scorer,
    UIConfig config,
    FateSelectorConfig selectorConfig,
    IUIService ui,
    IBrandingService branding
)
{
    public void Render()
    {
        var snapshot = fates.Snapshot();

        var fatesWithScores = snapshot
            .ToDictionary(f => f, scorer.Score)
            .OrderByDescending(kvp => (float)kvp.Value);

        foreach (var (fate, score) in fatesWithScores)
        {
            var groupState = ui.Render(Left(fate), Right(fate));
            if (groupState == ComposableGroupState.HoveredLeft)
            {
                ImGui.BeginTooltip();
                ui.Text($"Score: {score.Value:f2}");
                foreach (var (source, value) in score.Sources)
                {
                    ui.Text($" - {source}: {value:f2}");
                }

                ImGui.EndTooltip();
            }


            ui.ProgressBar(fate.Progress / 100f, new Vector2(-1, 8f));
        }
    }

    private ComposableGroup Left(Fate fate)
    {
        var selected = state.GetSelectedFate();
        var isSelected = selected != null && selected.Value == fate.Id;
        var shouldDo = selectorConfig.ShouldDoFate(fate);

        var group = ui.Compose();
        if (config.ShowFateTypeIcon)
        {
            group.Image(fate.IconId);
        }

        if (fate.IsBonus && config.ShowBonusFateIcon)
        {
            group.Image(60934);
        }

        if (fate.State == FateState.Preparation && config.ShowPreparingFateIcon)
        {
            group.Image(61397);
        }

        var labelColor = branding.Text;
        if (isSelected && config.HighlightSelectedFate)
        {
            labelColor = branding.ParsedBlue;
        }

        if (!shouldDo && config.FadeIgnoredFates)
        {
            labelColor = branding.DalamudGrey3;
        }

        group.Text(fate.Name, labelColor);

        return group;
    }

    private ComposableGroup Right(Fate fate)
    {
        var progress = fate.Progress;
        var progressLabel = $"{progress:0}%";

        if (config.ShowTimeEstimate)
        {
            var tracker = fate.ProgressTracker;
            if (tracker.Estimate() is { } eta)
            {
                var time = eta.CompletionTime - DateTimeOffset.Now;

                progressLabel = $"{time:mm\\:ss} ({eta.RSquared:f2}) | {progressLabel}";
            }
        }

        var group = ui.Compose();
        group.Text(progressLabel);

        return group;
    }
}
