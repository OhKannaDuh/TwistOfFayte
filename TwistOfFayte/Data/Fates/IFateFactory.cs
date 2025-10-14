using Dalamud.Game.ClientState.Fates;

namespace TwistOfFayte.Data.Fates;

public interface IFateFactory
{
    Fate Create(IFate context);
}
