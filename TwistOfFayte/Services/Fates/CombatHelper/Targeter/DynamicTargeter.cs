using System;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Lifecycle;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Data;

namespace TwistOfFayte.Services.Fates.CombatHelper.Targeter;

public class DynamicTargeter(
    IPlayer player,
    IServiceProvider services
) : ITargeter, IOnPreUpdate
{
    private uint lastJob = 0;

    private ITargeter? resolvedTargeter = null;

    private ITargeter? targeter
    {
        get => resolvedTargeter;
    }

    private ITargeter Resolve()
    {
        var classJob = player.GetClassJob();
        if (classJob == null)
        {
            return services.GetRequiredService<ClosestToPlayerTargeter>();
        }

        var id = classJob.Value.RowId;
        lastJob = id;

        if (player.IsCaster())
        {
            return services.GetRequiredService<CentroidTargeter>();
        }

        return services.GetRequiredService<ClosestToPlayerTargeter>();
    }

    public bool ShouldChange()
    {
        return targeter?.ShouldChange() ?? false;
    }

    public Target? GetTarget()
    {
        return targeter?.GetTarget();
    }

    public bool Contains(Target target)
    {
        return targeter?.Contains(target) ?? false;
    }

    public string Identify()
    {
        return targeter?.Identify() ?? "Unknown";
    }

    public void PreUpdate()
    {
        if (player.GetClassJob()?.RowId == lastJob)
        {
            return;
        }

        resolvedTargeter = Resolve();
    }
}
