using System.Numerics;

namespace TwistOfFayte.Data.Colliders;

public interface ICollider
{
    bool BlocksLineOfSight { get; }

    bool Contains(Vector2 p);

    bool IntersectsSegment(Vector2 a, Vector2 b);

    float Distance(Vector2 p);
}
