using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ocelot.Extensions;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public readonly record struct EnemyPoint(Vector3 Pos, float Hitbox);

public class CircularAoePositioner(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player
) : BaseCombatHelper(state, fates, npcs, player), IPositioner
{
    private readonly IPlayer player = player;

    private readonly INpcProvider npcs = npcs;

    private const float tankAndDancerRange = 5f;

    private const float healerRange = 8f;

    private bool IsHealer()
    {
        return player.GetClassJob()?.Role == 4;
    }

    private float GetRange()
    {
        return IsHealer() ? healerRange : tankAndDancerRange;
    }

    public bool ShouldMove()
    {
        return GetPosition().Distance2D(player.GetPosition()) >= 0.5f;
    }

    public Vector3 GetPosition()
    {
        var ranged = npcs.GetRangedEnemies().ToList();
        var melee = npcs.GetMeleeEnemies().ToList();

        if (ranged.Count == 0 && melee.Count == 0 || ranged.Count == 0)
        {
            return player.GetPosition();
        }

        var fate = GetFate();
        if (fate == null)
        {
            return player.GetPosition();
        }

        var aoeRadius = GetRange();

        var fatePosition = fate.Position;
        var fateRadius = fate.Radius;

        var candidates = new List<Vector3>
        {
            ClampToFate(player.GetPosition(), fatePosition, fateRadius),
        };

        var rangedList = ranged
            .Select(x => new EnemyPoint(x.Position, x.HitboxRadius))
            .ToList();

        candidates.AddRange(rangedList.Select(e => ClampToFate(e.Pos, fatePosition, fateRadius)));

        for (var i = 0; i < rangedList.Count; i++)
        {
            for (var j = i + 1; j < rangedList.Count; j++)
            {
                var Ri = aoeRadius + rangedList[i].Hitbox;
                var Rj = aoeRadius + rangedList[j].Hitbox;

                if (TryCircleIntersections(
                        rangedList[i].Pos,
                        Ri,
                        rangedList[j].Pos,
                        Rj,
                        out var p1,
                        out var p2))
                {
                    var c1 = ClampToFate(p1, fatePosition, fateRadius);
                    var c2 = ClampToFate(p2, fatePosition, fateRadius);
                    candidates.Add(c1);
                    candidates.Add(c2);
                }
            }
        }

        var playerPos = player.GetPosition().Truncate();

        int BestScore(Vector3 p)
        {
            return CoverageCount(p, aoeRadius, rangedList);
        }

        var best = candidates
            .DistinctBy(v => (Round(v.X, 3), Round(v.Z, 3)))
            .Select(p => new
            {
                Point = p,
                Score = BestScore(p),
                Dist = p.Distance2D(playerPos),
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Dist)
            .FirstOrDefault();

        if (best == null || best.Score == 0)
        {
            return ClampToFate(player.GetPosition(), fatePosition, fateRadius);
        }

        return best.Point;
    }

    public string Identify()
    {
        return "Circular Aoe";
    }

    private static Vector3 ClampToFate(Vector3 p, Vector3 center, float radius)
    {
        var dx = p.X - center.X;
        var dz = p.Z - center.Z;
        var d = MathF.Sqrt(dx * dx + dz * dz);
        if (d <= radius || d < 1e-4f)
        {
            return new Vector3(p.X, center.Y, p.Z);
        }

        var scale = radius / d;

        return new Vector3(center.X + dx * scale, center.Y, center.Z + dz * scale);
    }

    private static int CoverageCount(Vector3 pos, float aoeRadius, List<EnemyPoint> enemies)
    {
        var count = 0;
        foreach (var e in enemies)
        {
            if (pos.Distance2D(e.Pos) <= aoeRadius + e.Hitbox + 1e-3f)
            {
                count++;
            }
        }

        return count;
    }

    private static bool TryCircleIntersections(
        Vector3 c0,
        float r0,
        Vector3 c1,
        float r1,
        out Vector3 p1,
        out Vector3 p2)
    {
        p1 = default;
        p2 = default;

        var d = c0.Distance2D(c1);
        const float eps = 1e-4f;

        if (d < eps)
        {
            return false;
        }

        if (d > r0 + r1 + eps)
        {
            return false;
        }

        if (d < MathF.Abs(r0 - r1) - eps)
        {
            return false;
        }

        var a = (r0 * r0 - r1 * r1 + d * d) / (2f * d);
        var h2 = r0 * r0 - a * a;
        if (h2 < 0f)
        {
            h2 = 0f;
        }

        var h = MathF.Sqrt(h2);

        var vx = (c1.X - c0.X) / d;
        var vz = (c1.Z - c0.Z) / d;

        var mx = c0.X + a * vx;
        var mz = c0.Z + a * vz;

        var rx = -vz * h;
        var rz = vx * h;

        p1 = new Vector3(mx + rx, c0.Y, mz + rz);
        p2 = new Vector3(mx - rx, c0.Y, mz - rz);
        return true;
    }

    private static float Round(float v, int decimals)
    {
        var m = MathF.Pow(10f, decimals);
        return MathF.Round(v * m) / m;
    }
}
