using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Ocelot.Graphics;
using Ocelot.Lifecycle;
using Ocelot.Services.ClientState;
using Ocelot.Services.OverlayRenderer;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Debug;

public class DebugModule(IOverlayRenderer overlay, IPlayer player,  INpcProvider npcs,  DebugConfig config) : IOnRender
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
