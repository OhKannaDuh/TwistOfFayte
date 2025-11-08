using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Bindings.ImGui;
using Ocelot.Mechanic.Services;
using Ocelot.Pathfinding.Services;
using Ocelot.Rotation.Services;
using Ocelot.UI.Services;
using TwistOfFayte.Services.Fates.CombatHelper.Positioner;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;

namespace TwistOfFayte.Modules.Debug;

public class DynamicServiceDebugRenderable(
    IUIService ui,
    IBrandingService branding,
    // Ocelot Services
    IEnumerable<IPathfindingProvider> pathfindingProviders,
    IPathfindingPriorityService pathfindingPriority,
    IEnumerable<IRotationProvider> rotationProviders,
    IRotationPriorityService rotationPriority,
    IEnumerable<IMechanicProvider> mechanicProviders,
    IMechanicPriorityService mechanicPriority,
    // Tof Services
    IPositioner positioner,
    ITargeter targeter
) : IDebugRenderable
{
    public void Render()
    {
        ui.Text("Dynamic Service Providers:", branding.DalamudYellow);
        ImGui.Indent();
        ui.LabelledValue("Pathfinding Provider", PathfindingProvider());
        ui.LabelledValue("Rotation Provider", RotationProvider());
        ui.LabelledValue("Mechanic Provider", MechanicService());
        ui.LabelledValue("Positioner", positioner.Identify());
        ui.LabelledValue("Targeter", targeter.Identify());
        ImGui.Unindent();
    }

    private string PathfindingProvider()
    {
        var order = pathfindingPriority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = pathfindingProviders
            .Where(p => p.IsAvailable())
            .OrderBy(p => rank.GetValueOrDefault(p.InternalName, int.MaxValue))
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        return bestMatch?.DisplayName ?? "Unknown";
    }

    private string RotationProvider()
    {
        var order = rotationPriority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = rotationProviders
            .Where(p => p.IsAvailable())
            .OrderBy(p => rank.GetValueOrDefault(p.InternalName, int.MaxValue))
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        return bestMatch?.DisplayName ?? "Unknown";
    }

    private string MechanicService()
    {
        var order = mechanicPriority.GetPriority().ToList();
        var rank = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < order.Count; i++)
        {
            rank[order[i]] = i;
        }

        var bestMatch = mechanicProviders
            .Where(p => p.IsAvailable())
            .OrderBy(p => rank.GetValueOrDefault(p.InternalName, int.MaxValue))
            .ThenBy(p => p.InternalName, StringComparer.Ordinal)
            .FirstOrDefault();

        return bestMatch?.DisplayName ?? "Unknown";
    }
}
