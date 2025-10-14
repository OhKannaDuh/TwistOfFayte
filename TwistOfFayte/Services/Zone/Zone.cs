using System.Collections.Generic;
using System.Linq;
using Ocelot.Lifecycle;
using Ocelot.Services.ClientState;
using Ocelot.Services.Data;
using TwistOfFayte.Data.Zone;

namespace TwistOfFayte.Services.Zone;

public class Zone : IZone, IOnTerritoryChanged
{
    private readonly IDataRepository<AetheryteData> aetheryteRepository;

    public ushort Id { get; private set; } = 0;

    public IEnumerable<Aetheryte> Aetherytes { get; private set; } = [];

    public Zone(IDataRepository<AetheryteData> aetheryteRepository, IClient client)
    {
        this.aetheryteRepository = aetheryteRepository;
        UpdateTerritoryData(client.CurrentTerritoryId);
    }

    public void IOnTerritoryChanged(ushort territory)
    {
        if (territory == Id)
        {
            return;
        }

        UpdateTerritoryData(territory);
    }

    private void UpdateTerritoryData(ushort territory)
    {
        Id = territory;

        Aetherytes = aetheryteRepository
            .Where(a => a.Territory.RowId == territory && a.IsAetheryte)
            .Select(a => new Aetheryte(a));
    }
}
