using System.Collections.Generic;
using Dalamud.Plugin.Services;
using TwistOfFayte.Data;

namespace TwistOfFayte.Extensions;

public static class IEnumerableTargetExtensions
{
    extension(IEnumerable<Target> source)
    {
        public IEnumerable<Target> WhereBattleTarget(Target.BattleTargetFunc<bool> predicate, IObjectTable objects)
        {
            foreach (var e in source)
            {
                if (e.TryUse(predicate, objects, out var result) && result)
                {
                    yield return e;
                }
            }
        }

        public void ForEachBattleTarget(Target.BattleTargetAction action, IObjectTable objects)
        {
            foreach (var t in source)
            {
                t.TryUse(action, objects);
            }
        }
    }
}
