using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using ECommons.DalamudServices;

namespace TwistOfFayte.Modules.Tracker;

[OcelotModule]
public class TrackerModule(Plugin plugin, Config config) : Module(plugin, config)
{
    private readonly Dictionary<uint, Fate> _fates = [];

    public IReadOnlyDictionary<uint, Fate> Fates {
        get => _fates.OrderByDescending(f => f.Value.Score.Value).ToDictionary().AsReadOnly();
    }

    public override void Update(UpdateContext context)
    {
        var currentFates = Svc.Fates
            .Where(f => f.State is FateState.Preparation or FateState.Running)
            .Where(f => !f.GameData.Value.EventItem.IsValid) // Ignore collection fates
            .Where(f => f.Position != Vector3.Zero && f.Position != Vector3.NaN)
            .ToDictionary(f => (uint)f.FateId, f => f);

        foreach (var (id, data) in currentFates)
        {
            if (_fates.TryAdd(id, new Fate(data)))
            {
                // On Fate Spawned
            }
        }

        var despawned = _fates.Keys.Except(currentFates.Keys).ToList();
        foreach (var id in despawned)
        {
            _fates.Remove(id);
        }

        foreach (var fate in _fates.Values)
        {
            fate.Update(context.ForModule(this));
        }
    }
}
