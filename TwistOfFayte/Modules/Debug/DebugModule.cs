using System;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using Ocelot.Extensions;
using Ocelot.Graphics;
using Ocelot.Lifecycle;
using Ocelot.Services.OverlayRenderer;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Extensions;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Debug;

public class DebugModule(
    IOverlayRenderer overlay,
    IPlayer player,
    INpcProvider npcs,
    DebugConfig config,
    IObjectTable objects
) : IOnRender
{
    public void Render()
    {
        if (!config.ShowDebug)
        {
            return;
        }

        var playerPosition = player.GetPosition();

        if (config.ShouldShowDebugForStartNpc)
        {
            var startNpc = npcs.GetFateStartNpc();
            if (startNpc != null)
            {
                DrawLine(playerPosition, startNpc.Value.Position, new Color(0.0f, 1.0f, 0.4f));
            }
        }

        if (config.ShouldShowDebugForTurnInNpc)
        {
            var turnInNpc = npcs.GetFateTurnInNpc();
            if (turnInNpc != null)
            {
                DrawLine(playerPosition, turnInNpc.Value.Position, new Color(0.0f, 0.8f, 1.0f));
            }
        }

        var enemies = npcs.GetEnemies().ToList();

        if (config.ShouldShowDebugForEnemiesTargetingLocalPlayer)
        {
            var candidates = enemies.WhereBattleTarget(static (in t) => t.IsTargetingLocalPlayer(), objects);
            foreach (var enemy in candidates)
            {
                DrawLine(playerPosition, enemy.Position, new Color(1.0f, 0.2f, 0.2f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingNoPlayers)
        {
            var candidates = enemies.WhereBattleTarget(static (in t) => t.IsTargetingAnyPlayer(), objects);
            foreach (var enemy in candidates)
            {
                DrawLine(playerPosition, enemy.Position, new Color(0.8f, 0.8f, 0.8f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingAnotherPlayerWithoutTankStance)
        {
            var candidates = enemies.WhereBattleTarget(static (in t) => !t.IsTargetingLocalPlayer() && t.GetTargetedPlayer()?.HasTankStanceOn() == false,
                objects);
            foreach (var enemy in candidates)
            {
                DrawLine(playerPosition, enemy.Position, new Color(1.0f, 0.6f, 0.0f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingAnotherPlayerWithTankStance)
        {
            var candidates = enemies.WhereBattleTarget(static (in t) => !t.IsTargetingLocalPlayer() && t.GetTargetedPlayer()?.HasTankStanceOn() == true,
                objects);
            foreach (var enemy in candidates)
            {
                DrawLine(playerPosition, enemy.Position, new Color(0.3f, 0.4f, 1.0f));
            }
        }

        if (config.ShouldShowDebugForEnemyStartTethering)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.TryUse((in t) => t.GetSpawnPosition(), objects, out var spawn))
                {
                    continue;
                }

                var distance = spawn.Distance(enemy.Position);
                var max = enemy.WanderRange;

                var normalizedDistance = Math.Clamp(distance / max, 0f, 1f);
                var g = 1f - normalizedDistance;

                var color = new Color(normalizedDistance, g, 0f);

                DrawLine(spawn, enemy.Position, color);
            }
        }
    }

    private void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        if (!config.ShouldDrawLines)
        {
            return;
        }

        overlay.StrokeLine(from, to, color);
    }
}
