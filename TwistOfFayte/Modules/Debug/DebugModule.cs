using System.Linq;
using Dalamud.Interface.Colors;
using ECommons.GameHelpers;

namespace TwistOfFayte.Modules.Debug;

[OcelotModule]
public class DebugModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public override DebugConfig Config {
        get => PluginConfig.DebugConfig;
    }

    public override void Render(RenderContext context)
    {
        if (Config.RenderDistanceToEngagedEnemies && TargetHelper.InCombat.Any())
        {
            var maxDistance = TargetHelper.InCombat
                .Select(mob => Player.DistanceTo(mob) - mob.HitboxRadius)
                .Max();

            context.DrawCircle(Player.Position, maxDistance, ImGuiColors.DalamudRed);
        }

        if (Config.RenderDistanceToNotEngagedEnemies && TargetHelper.NotInCombat.Any())
        {
            context.DrawCircle(Player.Position, TargetHelper.NotInCombat.Max(Player.DistanceTo), ImGuiColors.HealerGreen);
        }

        if (Config.RenderAoeRadiusAroundPlayer)
        {
            context.DrawCircle(Player.Position, 5f, ImGuiColors.DalamudYellow);
        }

        if (Config.RenderLinetoEngagedEnemiesOutOfAoeRadius)
        {
            foreach (var mob in TargetHelper.InCombat.Where(mob => Player.DistanceTo(mob) > 5f + mob.HitboxRadius))
            {
                context.DrawLine(mob.Position, ImGuiColors.DPSRed);
            }
        }

        if (Config.RenderLineToNonEngagedEnemies)
        {
            foreach (var mob in TargetHelper.NotInCombat)
            {
                context.DrawLine(mob.Position, ImGuiColors.ParsedBlue);
            }
        }
    }
}
