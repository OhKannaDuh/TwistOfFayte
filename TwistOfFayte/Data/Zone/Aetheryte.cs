using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace TwistOfFayte.Data.Zone;

public class Aetheryte(AetheryteData data, Vector3 position)
{
    public readonly AetheryteData Data = data;

    public readonly uint Id = data.RowId;

    public Vector3 Position
    {
        get => position;
    }

    public unsafe bool Teleport()
    {
        return Telepo.Instance()->Teleport(Id, 0);
    }
}
