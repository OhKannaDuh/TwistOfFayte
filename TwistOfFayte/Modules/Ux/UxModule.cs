using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Ux;

public class UxModule(IPlayer player,  INpcProvider npcs, ITargeter targeter) : IOnPreUpdate
{
    public void PreUpdate()
    {
        
    }
}
