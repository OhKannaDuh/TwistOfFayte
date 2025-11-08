using System.Collections.Generic;
using Ocelot.Services.Pathfinding;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Data.Zone;
using TwistOfFayte.Services.Zone;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate;

public class TravellingToFateContext
{
    public Fate? fate { get; private set; }

    private Path? pathFromPlayer;

    private Dictionary<uint, Path> pathsFromAetherytes = [];

    public Path? chosenPath { get; private set; }

    public Aetheryte? chosenAetheryte { get; private set; }

    public void Clear()
    {
        fate = null;
        pathFromPlayer = null;
        pathsFromAetherytes = [];
        chosenPath = null;
        chosenAetheryte = null;
    }

    public void SetFate(Fate fate)
    {
        this.fate = fate;
    }

    public void SetPlayerPath(Path path)
    {
        pathFromPlayer = path;
    }

    public void AddAetherytePath(Aetheryte aetheryte, Path path)
    {
        pathsFromAetherytes.TryAdd(aetheryte.Id, path);
    }

    public void ChoosePath(IZone zone)
    {
        var bestCost = float.PositiveInfinity;
        Path? bestPath = null;
        Aetheryte? bestAetheryte = null;

        var directCost = Distance(pathFromPlayer);
        if (directCost < bestCost)
        {
            bestCost = directCost;
            bestPath = pathFromPlayer;
            bestAetheryte = null;
        }

        foreach (var aetheryte in zone.Aetherytes)
        {
            if (!pathsFromAetherytes.TryGetValue(aetheryte.Id, out var path))
            {
                continue;
            }

            var cost = Distance(path); // + TeleportPenalty;
            if (cost < bestCost)
            {
                bestCost = cost;
                bestPath = path;
                bestAetheryte = aetheryte;
            }
        }

        chosenPath = bestPath;
        chosenAetheryte = bestAetheryte;
    }

    public bool ShouldTeleport()
    {
        return chosenAetheryte != null;
    }

    private static float Distance(Path? p)
    {
        if (p is null)
        {
            return float.PositiveInfinity;
        }

        var d = p.Distance;
        return float.IsNaN(d) || d < 0 ? float.PositiveInfinity : d;
    }

    public void SetChosenPath(Path path)
    {
        chosenPath = path;
    }
}
