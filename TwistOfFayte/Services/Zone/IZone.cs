using System.Collections.Generic;
using TwistOfFayte.Data.Zone;

namespace TwistOfFayte.Services.Zone;

public interface IZone
{
    uint Id { get; }

    List<Aetheryte> Aetherytes { get; }
}
