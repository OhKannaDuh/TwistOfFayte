using System.Collections.Generic;
using TwistOfFayte.Data;

namespace TwistOfFayte.Services.Npc;

public interface INpcProvider
{
    Target? GetFateStartNpc();

    Target? GetFateTurnInNpc();

    IEnumerable<Target> GetEnemies();

    IEnumerable<Target> GetRangedEnemies();

    IEnumerable<Target> GetMeleeEnemies();

    IEnumerable<Target> GetForlornMaidens();

    IEnumerable<Target> GetNonFateNpcs();
}
