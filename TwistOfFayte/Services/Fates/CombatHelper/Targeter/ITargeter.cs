using TwistOfFayte.Data;

namespace TwistOfFayte.Services.Fates.CombatHelper.Targeter;

public interface ITargeter
{
    bool ShouldChange();

    Target? GetTarget();

    bool Contains(Target target);

    string Identify();
}
