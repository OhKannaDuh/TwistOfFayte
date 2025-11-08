using System.Numerics;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;

namespace TwistOfFayte.Data;

public class PlayerTracker(IPlayer player) : IOnUpdate
{
    public Vector3 Position { get; private set; } = Vector3.NaN;

    public void Update()
    {
        Position = player.GetPosition();
    }
}
