using System;
using System.Numerics;

namespace TwistOfFayte.Data.Colliders;

public class AabbCollider(Vector2 min, Vector2 max, bool blocksLineOfSight = true) : ICollider
{
    private Vector2 Min { get; } = Vector2.Min(min, max);

    private Vector2 Max { get; } = Vector2.Max(min, max);


    public bool BlocksLineOfSight { get; } = blocksLineOfSight;

    public bool Contains(Vector2 p)
    {
        return p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y;
    }

    public bool IntersectsSegment(Vector2 a, Vector2 b)
    {
        var d = b - a;

        var t0 = 0f;
        var t1 = 1f;

        bool Clip(float p, float q)
        {
            if (p == 0)
            {
                return q >= 0;
            }

            var r = q / p;
            if (p < 0)
            {
                if (r > t1)
                {
                    return false;
                }

                if (r > t0)
                {
                    t0 = r;
                }
            }
            else
            {
                if (r < t0)
                {
                    return false;
                }

                if (r < t1)
                {
                    t1 = r;
                }
            }

            return true;
        }

        if (!Clip(-d.X, a.X - Min.X))
        {
            return false;
        }

        if (!Clip(d.X, Max.X - a.X))
        {
            return false;
        }

        if (!Clip(-d.Y, a.Y - Min.Y))
        {
            return false;
        }

        if (!Clip(d.Y, Max.Y - a.Y))
        {
            return false;
        }

        return t0 <= t1 && t0 <= 1f && t1 >= 0f;
    }

    public float Distance(Vector2 p)
    {
        var dx = MathF.Max(MathF.Max(Min.X - p.X, 0f), p.X - Max.X);
        var dy = MathF.Max(MathF.Max(Min.Y - p.Y, 0f), p.Y - Max.Y);

        return MathF.Sqrt(dx * dx + dy * dy);
    }
}
