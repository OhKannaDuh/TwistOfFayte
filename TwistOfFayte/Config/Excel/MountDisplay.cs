using System.Globalization;
using Lumina.Excel.Sheets;
using Ocelot.Config.Renderers.Excel;

namespace TwistOfFayte.Config.Excel;

public class MountDisplay : IExcelDisplay<Mount>
{
    public string Display(Mount row)
    {
        try
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(row.Singular.ToString());
        }
        catch
        {
            return "Unknown Mount"; // @todo translate
        }
    }
}
