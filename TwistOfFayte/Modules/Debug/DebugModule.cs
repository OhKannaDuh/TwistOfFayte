using System;
using System.Linq;
using System.Numerics;
using Ocelot.Extensions;
using Ocelot.Graphics;
using Ocelot.Lifecycle;
using Ocelot.Services.OverlayRenderer;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Debug;

public class DebugModule(IOverlayRenderer overlay, IPlayer player, INpcProvider npcs, DebugConfig config) : IOnRender
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
                DrawLine(playerPosition, startNpc.Position, new Color(0.0f, 1.0f, 0.4f));
            }
        }

        if (config.ShouldShowDebugForTurnInNpc)
        {
            var turnInNpc = npcs.GetFateTurnInNpc();
            if (turnInNpc != null)
            {
                DrawLine(playerPosition, turnInNpc.Position, new Color(0.0f, 0.8f, 1.0f));
            }
        }

        var enemies = npcs.GetEnemies().ToList();

        if (config.ShouldShowDebugForEnemiesTargetingLocalPlayer)
        {
            foreach (var enemy in enemies.Where(e => e.IsTargetingLocalPlayer()))
            {
                DrawLine(playerPosition, enemy.Position, new Color(1.0f, 0.2f, 0.2f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingNoPlayers)
        {
            foreach (var enemy in enemies.Where(e => !e.IsTargetingAnyPlayer()))
            {
                DrawLine(playerPosition, enemy.Position, new Color(0.8f, 0.8f, 0.8f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingAnotherPlayerWithoutTankStance)
        {
            foreach (var enemy in enemies.Where(e => !e.IsTargetingLocalPlayer() && e.GetTargetedPlayer()?.HasTankStanceOn() == false))
            {
                DrawLine(playerPosition, enemy.Position, new Color(1.0f, 0.6f, 0.0f));
            }
        }

        if (config.ShouldShowDebugForEnemiesTargetingAnotherPlayerWithTankStance)
        {
            foreach (var enemy in enemies.Where(e => !e.IsTargetingLocalPlayer() && e.GetTargetedPlayer()?.HasTankStanceOn() == true))
            {
                DrawLine(playerPosition, enemy.Position, new Color(0.3f, 0.4f, 1.0f));
            }
        }

        if (config.ShouldShowDebugForEnemyStartTethering)
        {
            foreach (var enemy in enemies)
            {
                var spawn = enemy.GetSpawnPosition();
                var distance = spawn.Distance(enemy.Position);
                var max = 40f;

                var normalizedDistance = Math.Clamp(distance / max, 0f, 1f);
                var r = normalizedDistance;
                var g = 1f - normalizedDistance;
                var b = 0f;

                var color = new Color(r, g, b);

                DrawLine(enemy.GetSpawnPosition(), enemy.Position, color);
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
