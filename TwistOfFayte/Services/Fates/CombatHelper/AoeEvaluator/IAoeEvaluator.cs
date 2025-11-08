using TwistOfFayte.Data;

namespace TwistOfFayte.Services.Fates.CombatHelper.AoeEvaluator;

public interface IAoeEvaluator
{
    bool Contains(Target target);
}
