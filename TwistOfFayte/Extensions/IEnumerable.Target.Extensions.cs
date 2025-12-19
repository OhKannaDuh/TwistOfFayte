using System.Collections.Generic;
using TwistOfFayte.Data;

namespace TwistOfFayte.Extensions;

public static class IEnumerableTargetExtensions
{
    public static IEnumerable<Target> WhereBattleTarget(this IEnumerable<Target> source, Target.BattleTargetFunc<bool> predicate)
    {
        foreach (var e in source)
        {
            if (e.TryUse(predicate, out var result) && result)
            {
                yield return e;
            }
        }
    }
    
    public static void ForEachBattleTarget(this IEnumerable<Target> source, Target.BattleTargetAction action)
    {
        foreach (var t in source)
        {
            t.TryUse(action);
        }
    }
}
