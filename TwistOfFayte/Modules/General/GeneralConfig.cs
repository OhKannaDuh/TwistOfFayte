using Lumina.Excel.Sheets;

namespace TwistOfFayte.Modules.General;

public class GeneralConfig : ModuleConfig
{
    [ExcelSheet(typeof(Mount), nameof(MountProvider))]
    [Searchable]
    public uint Mount { get; set; } = 1;

    [Checkbox]
    public bool MountRoulette { get; set; } = false;

    [Checkbox]
    public bool ShouldExtractMateria { get; set; } = false;

    [Checkbox]
    public bool ShouldRepairGear { get; set; } = false;

    [IntRange(0, 16)]
    public int MaxMobsToFight { get; set; } = 0;
}
