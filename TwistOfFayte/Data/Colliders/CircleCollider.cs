using System;
using System.Numerics;

namespace TwistOfFayte.Data.Colliders;

public class CircleCollider(Vector2 center, float radius, bool blocksLineOfSight = true) : ICollider
{
    private Vector2 Center { get; } = center;

    private float Radius { get; } = radius;


    public bool BlocksLineOfSight { get; } = blocksLineOfSight;

    public bool Contains(Vector2 p)
    {
        return Vector2.DistanceSquared(p, Center) <= Radius * Radius;
    }

    public bool IntersectsSegment(Vector2 a, Vector2 b)
    {
        var d = b - a;
        var f = a - Center;

        var A = Vector2.Dot(d, d);
        var B = 2 * Vector2.Dot(f, d);
        var C = Vector2.Dot(f, f) - Radius * Radius;

        var disc = B * B - 4 * A * C;
        if (disc < 0)
        {
            return false;
        }

        var sqrt = MathF.Sqrt(disc);
        var t1 = (-B - sqrt) / (2 * A);
        var t2 = (-B + sqrt) / (2 * A);
        return t1 >= 0 && t1 <= 1 || t2 >= 0 && t2 <= 1;
    }

    public float Distance(Vector2 p)
    {
        return MathF.Max(0f, Vector2.Distance(p, Center) - Radius);
    }
}
