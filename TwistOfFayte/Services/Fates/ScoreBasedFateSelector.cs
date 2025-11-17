using System;
using System.Linq;
using Ocelot.Lifecycle;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates;

public class ScoreBasedFateSelector(
    IFateRepository fates,
    IFateScorer scorer,
    IStateManager state,
    FateSelectorConfig config,
    FateBlacklist blacklist
) : IFateSelector, IOnUpdate
{
    public event Action<FateId>? SelectionChanged;

    public FateId? Select()
    {
        return fates.Snapshot()
            .Where(f => config.ShouldDoFate(f, blacklist))
            .OrderByDescending(f => (float)scorer.Score(f)).FirstOrDefault()?.Id;
    }

    public void Update()
    {
        if (!state.IsActive())
        {
            return;
        }

        var selectedFateId = state.GetSelectedFate();
        if (selectedFateId != null)
        {
            var selectedFate = fates.Snapshot().FirstOrDefault(f => f.Id == selectedFateId);
            if (selectedFate != null && config.ShouldDoFate(selectedFate, blacklist))
            {
                return;
            }
        }

        var selected = Select();
        if (selected == null)
        {
            return;
        }

        state.SetSelectedFate(selected.Value);
        SelectionChanged?.Invoke(selected.Value);
    }
}
