using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Ocelot.Config.Renderers.Excel;

namespace TwistOfFayte.Config.Excel;

public class ZoneFilter : IExcelFilter<TerritoryType>
{
    public bool Filter(TerritoryType row)
    {
        unsafe
        {
            return Telepo.Instance()->TeleportList.Any(t => t.TerritoryId == row.RowId);
        }
    }
}
