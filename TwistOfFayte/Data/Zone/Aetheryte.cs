using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Map = ECommons.GameHelpers.Map;

namespace TwistOfFayte.Data.Zone;

public class Aetheryte(AetheryteData data)
{
    public readonly AetheryteData Data = data;
    
    public readonly uint Id = data.RowId;

    public Vector3 Position {
        get => Map.AetherytePosition(Data.RowId);
    }

    

    public unsafe void Teleport()
    {
        Telepo.Instance()->Teleport(Id, 0);
    }
}
