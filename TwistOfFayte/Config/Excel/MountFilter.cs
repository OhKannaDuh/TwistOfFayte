using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Ocelot.Config.Renderers.Excel;

namespace TwistOfFayte.Config.Excel;

public class MountFilter : IExcelFilter<Mount>
{
    public unsafe bool Filter(Mount row)
    {
        return PlayerState.Instance()->IsMountUnlocked(row.RowId);
    }
}
