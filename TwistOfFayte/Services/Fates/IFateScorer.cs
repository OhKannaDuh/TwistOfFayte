using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Services.Fates;

public interface IFateScorer
{
    FateScore Score(Fate fate);
}
