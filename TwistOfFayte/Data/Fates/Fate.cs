using System;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using Lumina.Excel.Sheets;
using Ocelot.Services.Data;

namespace TwistOfFayte.Data.Fates;

public class Fate
{
    public readonly FateId Id;

    public Vector3 Position { get; private set; }

    public FateState State { get; private set; }

    public readonly string Name;

    public readonly float Radius;

    public readonly bool IsBonus;

    public readonly int MaxLevel;

    public readonly uint IconId;

    public readonly FateType Type;

    public byte Progress { get; private set; }

    public readonly FateProgressTracker ProgressTracker = new();

    public readonly FateObjectiveTracker ObjectiveTracker;

    public readonly FateData GameData;

    public readonly EventItem? EventItem;

    public Fate(FateId id, IFate context, IDataRepository<FateData> fateDataRepository)
    {
        Id = id;
        Position = context.Position;
        State = context.State;
        Radius = context.Radius;
        Name = context.Name.ToString();
        IsBonus = context.HasBonus;
        MaxLevel = context.MaxLevel;
        IconId = context.IconId;
        Type = Enum.IsDefined(typeof(FateType), context.IconId) ? (FateType)context.IconId : FateType.Unknown;

        Progress = context.Progress;
        ObjectiveTracker = new FateObjectiveTracker(Progress);

        GameData = fateDataRepository.Get(context.FateId);
        if (GameData.EventItem.IsValid)
        {
            EventItem = GameData.EventItem.Value;
        }
    }

    public void Update(IFate context)
    {
        Position = context.Position;
        State = context.State;
        Progress = context.Progress;

        ProgressTracker.Observe(this);

        if (ShouldTrackObjectiveEstimate())
        {
            ObjectiveTracker.Observe(this);
        }
    }

    public bool ShouldTrackObjectiveEstimate()
    {
        return Type is FateType.Mobs or FateType.Defend or FateType.Collect;
    }
}
