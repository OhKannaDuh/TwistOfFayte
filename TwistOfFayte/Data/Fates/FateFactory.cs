using Dalamud.Game.ClientState.Fates;
using Ocelot.Services.Data;

namespace TwistOfFayte.Data.Fates;

public class FateFactory(IDataRepository<FateData> fateDataRepository) : IFateFactory
{
    public Fate Create(IFate context)
    {
        var id = new FateId(context.FateId);

        return new Fate(id, context, fateDataRepository);
    }
}
