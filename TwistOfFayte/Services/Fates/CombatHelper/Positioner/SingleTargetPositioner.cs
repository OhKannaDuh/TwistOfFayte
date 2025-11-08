using System.Linq;
using System.Numerics;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Data;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public class SingleTargetPositioner(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player
) : BaseCombatHelper(state, fates, npcs, player), IPositioner
{
    private readonly IPlayer player = player;

    private Target? GetTarget()
    {
        return GetNpcsTargetingLocalPlayer().FirstOrDefault();
    }

    public bool ShouldMove()
    {
        var target = GetTarget();
        if (target == null)
        {
            return false;
        }

        var distance = target.Position.Distance2D(player.GetPosition()) - target.GameObject.HitboxRadius;

        return distance > player.GetRange() || distance < target.GameObject.HitboxRadius;
    }

    public Vector3 GetPosition()
    {
        var target = GetTarget();
        var fate = GetFate();
        if (target == null || fate == null)
        {
            return player.GetPosition();
        }

        var playerPosition = player.GetPosition();

        var targetPosition = target.Position;
        var targetRadius = target.GameObject.HitboxRadius;

        var playerDistance = playerPosition.Distance2D(targetPosition) - targetRadius;
        var playerRange = player.GetRange();

        if (playerDistance < targetRadius)
        {
            const float margin = 0.75f;
            Vector2 dir;

            if (playerDistance > 1e-5f)
            {
                dir = Vector2.Normalize(playerPosition.Truncate() - targetPosition.Truncate());
            }
            else
            {
                dir = new Vector2(1f, 0f);
            }

            var out2 = targetPosition.Truncate() + dir * (targetRadius + margin);

            return new Vector3(out2.X, playerPosition.Y, out2.Y);
        }

        if (playerDistance <= playerRange)
        {
            return playerPosition;
        }

        var position = target.Position.GetApproachPosition(player.GetPosition(), playerRange);

        var fatePosition = fate.Position.Truncate();
        var fateRadius = fate.Radius;
        var position2d = position.Truncate();

        var offset = position2d - fatePosition;
        if (offset.Length() > fateRadius)
        {
            offset = offset.Normalized() * fateRadius;
            position2d = fatePosition + offset;
        }

        return position2d.Extend(position.Y);
    }

    public string Identify()
    {
        return "Single Target";
    }
}
