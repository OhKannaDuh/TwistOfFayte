using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameHelpers;

namespace TwistOfFayte;

public class Path : IDisposable
{
    private readonly Vector3 destination;

    private readonly VNavmesh vnavmesh;

    private readonly bool fly;

    private readonly Func<List<Vector3>, List<Vector3>> callback;

    private Action<Vector3>? watcher;

    public bool HasRunCallback = false;

    private readonly Task<List<Vector3>> Task;

    public bool IsDone {
        get => (HasRunCallback || IsCancelled) && !vnavmesh.IsRunning();
    }

    public bool IsCancelled {
        get => Task.IsCanceled;
    }

    private Path(Vector3 destination, VNavmesh vnavmesh, bool fly, Func<List<Vector3>, List<Vector3>> callback)
    {
        this.destination = destination;
        this.vnavmesh = vnavmesh;
        this.fly = fly;
        this.callback = callback;

        Task = vnavmesh.Pathfind(Player.Position, destination, fly);

        Svc.Framework.Update += Update;
    }

    public static Path Walk(Vector3 destination, VNavmesh vnavmesh)
    {
        return Walk(destination, vnavmesh, path => path);
    }

    public static Path Walk(Vector3 destination, VNavmesh vnavmesh, Func<List<Vector3>, List<Vector3>> ProcessPath)
    {
        return new Path(destination, vnavmesh, false, ProcessPath);
    }

    public static Path Fly(Vector3 destination, VNavmesh vnavmesh)
    {
        return Fly(destination, vnavmesh, path => path);
    }

    public static Path Fly(Vector3 destination, VNavmesh vnavmesh, Func<List<Vector3>, List<Vector3>> ProcessPath)
    {
        return new Path(destination, vnavmesh, true, ProcessPath);
    }

    public Path WithWatcher(Action<Vector3> watcher)
    {
        this.watcher = watcher;
        return this;
    }

    private void Update(IFramework _)
    {
        if (HasRunCallback)
        {
            if (vnavmesh.IsRunning() && watcher != null)
            {
                watcher.Invoke(destination);
            }

            return;
        }

        if (!Task.IsCompleted)
        {
            return;
        }

        if (Task.IsCanceled || Task.IsFaulted)
        {
            throw new TaskCanceledException();
        }

        vnavmesh.Stop();

        if (!vnavmesh.IsRunning())
        {
            HasRunCallback = true;
            var path = callback.Invoke(Task.Result);
            vnavmesh.MoveTo(path, fly);
        }
    }

    public void Dispose()
    {
        Task.Dispose();
        Svc.Framework.Update -= Update;
    }
}
