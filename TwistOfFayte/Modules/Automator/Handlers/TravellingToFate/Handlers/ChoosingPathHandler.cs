using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Ocelot.Ipc.VNavmesh;
using Ocelot.Services.Logger;
using Ocelot.Services.Pathfinding;
using Ocelot.Services.PlayerState;
using Ocelot.States.Flow;
using TwistOfFayte.Services.Fates;
using TwistOfFayte.Services.State;
using TwistOfFayte.Services.Zone;

namespace TwistOfFayte.Modules.Automator.Handlers.TravellingToFate.Handlers;

public class ChoosingPathHandler(
    IPathfinder pathfinder,
    IPlayer player,
    IZone zone,
    IFateRepository fates,
    IStateManager state,
    TravellingToFateContext context,
    IVNavmeshIpc vnavmesh,
    ILogger logger
) : FlowStateHandler<TravellingToFateState>(TravellingToFateState.ChoosingPath)
{
    private readonly List<Task<Path>> paths = [];

    private Task? watchAll;

    public override void Enter()
    {
        base.Enter();
        context.Clear();

        var selected = state.GetSelectedFate();
        if (selected == null)
        {
            logger.Error("No fate selected");
            return;
        }

        if (!fates.HasFate(selected.Value))
        {
            logger.Error("Fate not found in repository");
            return;
        }

        var list = fates.Snapshot();
        var fate = list.FirstOrDefault(f => f.Id == selected.Value);
        if (fate == null)
        {
            logger.Error("Fate not found in repository");
            return;
        }

        context.SetFate(fate);

        var destination = fate.Position;
        var playerDistance = Vector3.Distance(player.GetPosition(), destination);

        paths.Add(pathfinder.Pathfind(new PathfinderConfig(destination)
        {
            From = player.GetPosition(),
            AllowFlying = true,
        }));

        // Only check the two closest aetherytes as the crow flies
        // And only if there are less than double the players distance to the fate
        var aetherytes = zone.Aetherytes.OrderBy(a => Vector3.Distance(a.Position, destination)).Take(2);
        foreach (var aetheryte in aetherytes)
        {
            if (Vector3.Distance(aetheryte.Position, destination) > playerDistance * 2f)
            {
                continue;
            }

            paths.Add(pathfinder.Pathfind(new PathfinderConfig(destination)
            {
                From = vnavmesh.FindPointOnFloor(aetheryte.Position, 5f),
                AllowFlying = true,
            }));
        }

        watchAll = Task.WhenAll(paths);
        logger.Info("Kicked off {count} pathfinding tasks", paths.Count);
    }

    public override TravellingToFateState? Handle()
    {
        if (!state.IsActive() || watchAll == null)
        {
            return TravellingToFateState.Arrived;
        }

        if (!watchAll.IsCompleted)
        {
            return null;
        }

        AddPathsToContext();
        context.ChoosePath(zone);

        return context.ShouldTeleport() ? TravellingToFateState.Teleporting : TravellingToFateState.Mounting;
    }

    private void AddPathsToContext()
    {
        var count = 0;
        foreach (var task in paths)
        {
            if (!task.IsCompletedSuccessfully)
            {
                continue;
            }

            var path = task.Result;
            if (path.Nodes.Count == 0)
            {
                continue;
            }

            var start = path.Nodes.First();
            if (Vector3.Distance(start, player.GetPosition()) <= 5f)
            {
                count++;
                context.SetPlayerPath(path);
            }
            else
            {
                foreach (var aetheryte in zone.Aetherytes)
                {
                    if (Vector3.Distance(start, aetheryte.Position) <= 20f)
                    {
                        count++;
                        context.AddAetherytePath(aetheryte, path);
                        break;
                    }
                }
            }
        }

        logger.Info("Assigned {count} paths", count);
    }
}
