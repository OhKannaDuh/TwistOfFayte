using System;
using System.Linq;
using Ocelot.Lifecycle;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates;

public class ScoreBasedFateSelector(IFateRepository fates, IFateScorer scorer, IStateManager state, FateSelectorConfig config) : IFateSelector, IOnUpdate
{
    public event Action<FateId>? SelectionChanged;

    public FateId? Select()
    {
        return fates.Snapshot()
            .Where(config.ShouldDoFate)
            .OrderByDescending(f => (float)scorer.Score(f)).FirstOrDefault()?.Id;
    }

    public void Update()
    {
        if (!state.IsActive() || state.GetSelectedFate() != null)
        {
            return;
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
