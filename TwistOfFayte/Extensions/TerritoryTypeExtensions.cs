using System;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons.DalamudServices;
using Lumina.Data.Files;
using Lumina.Excel.Sheets;
using TwistOfFayte.Data;

namespace TwistOfFayte.Extensions;

public static class TerritoryTypeExtensions
{
    private static bool TryGetBasePath(this TerritoryType territory, out string path)
    {
        path = null!;
        var bg = territory.Bg.ExtractText();
        if (bg.IsNullOrEmpty())
        {
            return false;
        }

        bg = bg.Replace("\\", "/");

        const string BgPrefix = "bg/";
        if (bg.StartsWith(BgPrefix, StringComparison.Ordinal))
        {
            bg = bg[BgPrefix.Length..];
        }

        const string Marker = "/level/";
        var index = bg.IndexOf(Marker, StringComparison.Ordinal);
        if (index < 0)
        {
            return false;
        }

        path = BgPrefix + bg[..(index + 1)];

        if (path.EndsWith("/"))
        {
            path = path[..^1];
        }

        return true;
    }

    public static bool TryGetLgbFilePath(this TerritoryType territory, LgbType type, out string path)
    {
        path = null!;

        if (!territory.TryGetBasePath(out var basePath))
        {
            return false;
        }

        path = $"{basePath}/level/{type.GetFileName()}";
        return true;
    }

    public static bool TryGetLgbFile(this TerritoryType territory, LgbType type, IDataManager data, out LgbFile file)
    {
        file = null!;

        if (!territory.TryGetLgbFilePath(type, out var path))
        {
            return false;
        }

        var fileOpt = data.GetFile<LgbFile>(path);
        if (fileOpt == null)
        {
            return false;
        }

        file = fileOpt;
        return true;
    }
}
