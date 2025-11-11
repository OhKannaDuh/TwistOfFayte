using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Ocelot.Lifecycle;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Fates.CombatHelper.Targeter;
using TwistOfFayte.Services.Npc;

namespace TwistOfFayte.Modules.Ux;

public class UxModule(INpcProvider npcs, ITargeter targeter, UXConfig config) : IOnPreUpdate
{
    private bool wasHighlightMobsEnabled = config.HighlightMobs;

    public void PreUpdate()
    {
        if (config.HighlightMobs)
        {
            foreach (var npc in npcs.GetEnemies())
            {
                var color = targeter.Contains(npc) ? ObjectHighlightColor.Green : ObjectHighlightColor.Red;
                npc.Highlight(color);
            }
        }

        if (wasHighlightMobsEnabled != config.HighlightMobs)
        {
            wasHighlightMobsEnabled = config.HighlightMobs;
            if (!wasHighlightMobsEnabled)
            {
                foreach (var npc in npcs.GetEnemies())
                {
                    npc.Highlight(ObjectHighlightColor.None);
                }
            }
        }
    }
}
