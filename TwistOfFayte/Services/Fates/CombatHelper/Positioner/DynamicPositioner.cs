using System;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Npc;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Services.Fates.CombatHelper.Positioner;

public class DynamicPositioner(
    IStateManager state,
    IFateRepository fates,
    INpcProvider npcs,
    IPlayer player,
    SingleTargetPositioner singleTargetPositioner,
    NullPositioner nullPositioner,
    IServiceProvider services,
    CombatConfig combatConfig
) : BaseCombatHelper(state, fates, npcs, player), IPositioner, IOnPreUpdate
{
    private readonly IPlayer player = player;

    private uint lastJob = 0;

    private int TargetCount
    {
        get => GetNpcsTargetingLocalPlayer().Count();
    }

    private bool ShouldSingleTarget
    {
        get => TargetCount == 1;
    }

    private IPositioner? resolvedPositioner = null;

    private IPositioner? positioner
    {
        get
        {
            if (combatConfig.PreventMovementWhileFightingGatheredMobs)
            {
                return nullPositioner;
            }

            return ShouldSingleTarget ? singleTargetPositioner : resolvedPositioner;
        }
    }

    private IPositioner Resolve()
    {
        if (combatConfig.PreventMovementWhileFightingGatheredMobs)
        {
            return services.GetRequiredService<NullPositioner>();
        }

        var classJob = player.GetClassJob();
        if (classJob == null)
        {
            return services.GetRequiredService<CircularAoePositioner>();
        }

        var id = classJob.Value.RowId;
        lastJob = id;

        if (player.IsCaster())
        {
            return services.GetRequiredService<CasterAoePositioner>();
        }

        return services.GetRequiredService<CircularAoePositioner>();
    }


    public bool ShouldMove()
    {
        return positioner?.ShouldMove() ?? false;
    }

    public Vector3 GetPosition()
    {
        return positioner?.GetPosition() ?? player.GetPosition();
    }

    public string Identify()
    {
        return positioner?.Identify() ?? "Unknown";
    }

    public void PreUpdate()
    {
        if (player.GetClassJob()?.RowId == lastJob)
        {
            return;
        }

        resolvedPositioner = Resolve();
    }
}
