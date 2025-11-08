using System;
using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Services.Fates;

public interface IFateSelector
{
    event Action<FateId>? SelectionChanged;

    FateId? Select();
}
