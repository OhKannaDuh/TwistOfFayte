using System;
using System.Globalization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ExcelMount = Lumina.Excel.Sheets.Mount;

namespace TwistOfFayte.Modules.General;

public class MountProvider : ExcelSheetItemProvider<ExcelMount>
{
    public override unsafe bool Filter(ExcelMount item)
    {
        return PlayerState.Instance()->IsMountUnlocked(item.RowId);
    }

    public override string GetLabel(ExcelMount item)
    {
        try
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Singular.ToString());
        }
        catch (Exception)
        {
            return "Unknown Mount"; // @todo translate
        }
    }
}
