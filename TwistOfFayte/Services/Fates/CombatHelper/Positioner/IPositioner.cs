using System.Numerics;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public interface IPositioner
{
    bool ShouldMove();

    Vector3 GetPosition();

    string Identify();
}
