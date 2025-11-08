using System.Globalization;
using Lumina.Excel.Sheets;
using Ocelot.Config.Renderers.Excel;

namespace TwistOfFayte.Config.Excel;

public class ZoneDisplay : IExcelDisplay<TerritoryType>
{
    public string Display(TerritoryType row)
    {
        return row.PlaceName.Value.Name.ToString();
    }
}
