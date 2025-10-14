using System;
using System.Collections.Generic;
using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Services.Fates;

public interface IFateRepository
{
    event Action<Fate> FateAdded;

    event Action<FateId> FateRemoved;

    IReadOnlyList<Fate> Snapshot();

    bool HasFate(FateId id);
}
