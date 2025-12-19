using System.Numerics;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public class NullPositioner : IPositioner
{
    public bool ShouldMove()
    {
        return false;
    }

    public Vector3 GetPosition()
    {
        return Vector3.NaN;
    }

    public string Identify()
    {
        return "Null Positioner";
    }
}
