using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using TwistOfFayte.Modules.State.Handlers;
using TwistOfFayte.Modules.Tracker;
using TwistOfFayte.Zone;
using FateState = Dalamud.Game.ClientState.Fates.FateState;

namespace TwistOfFayte;

public class Fate : IEquatable<Fate>
{
    public readonly ushort Id;

    public readonly Vector3 Position;

    public readonly float Radius;

    public readonly string Name;

    public readonly bool IsBonus;

    public readonly int MaxLevel;

    public readonly FateProgress ProgressTracker = new();

    public readonly List<FateContext.FateObjective> Objectives = [];

    public readonly Score Score = new();

    private unsafe Fate(FateContext* context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Id = context->FateId;
        Position = context->Location;
        Radius = context->Radius;
        Name = context->Name.ToString();
        IsBonus = context->IsBonus;
        MaxLevel = context->MaxLevel;

        if (Position == Vector3.Zero || Position == Vector3.NaN)
        {
            throw new ArgumentException("Fate position was invalid");
        }
    }

    public unsafe Fate(IFate fate) : this((FateContext*)fate.Address) { }

    public static unsafe Fate Current()
    {
        if (!FateHelper.IsInFate())
        {
            throw new Exception("There is no current fate.");
        }

        return new Fate(FateManager.Instance()->CurrentFate);
    }

    private IFate? Data {
        get => Svc.Fates.FirstOrDefault(fate => fate.FateId == Id);
    }

    public bool IsActive {
        get => Data is { State: FateState.Preparation } or { State: FateState.Running };
    }

    public FateState State {
        get => Data?.State ?? FateState.Ended;
    }

    public byte Progress {
        get => Data?.Progress ?? 0;
    }

    public void Update(UpdateContext context)
    {
        UpdateScore(context);

        if (Progress <= 0)
        {
            return;
        }

        if (ProgressTracker.Count == 0 || ProgressTracker.Latest != Progress)
        {
            ProgressTracker.Add(Progress);
        }
    }


    public void UpdateScore(UpdateContext context)

    {
        if (!context.IsForModule<TrackerModule>(out var module))
        {
            return;
        }

        if (context.Plugin is Plugin plugin && IsBlacklisted(plugin))
        {
            return;
        }

        var config = module.SelectorModule.Config;

        Score.Clear();

        if (IsCurrent())
        {
            Score.Add("Current", 1024f);
            return;
        }

        var aetheryteDistance = ZoneHelper.GetAetherytes()
            .Select(a => Vector3.Distance(Position, a.Position))
            .Order()
            .FirstOrDefault(float.MaxValue);

        var playerDistance = Vector3.Distance(Position, Player.Position);

        var distance = Math.Min(aetheryteDistance, playerDistance);

        Score.Add("Distance", (2048 - distance) / 25f);

        var teleportRequired = aetheryteDistance < playerDistance;
        if (teleportRequired)
        {
            Score.Add("Teleport Time", -(config.TimeToTeleport * config.CostPerYalm));
        }

        if (IsBonus)
        {
            Score.Add("Bonus Modifier", config.BonusFateModifier);
        }

        var estimate = ProgressTracker.EstimateTimeToCompletion();
        if (estimate == null)
        {
            Score.Add("Unstarted Modifier", config.UnstartedFateModifier);
        }
        else
        {
            var timeLeft = (float)estimate.Value.TotalSeconds;
            var timeToReach = distance / config.CostPerYalm;
            if (teleportRequired)
            {
                timeToReach += config.TimeToTeleport;
            }

            // Less than about 30 seconds left when we would arrive
            if (timeToReach > timeLeft - config.TimeRequiredToConsiderFate)
            {
                Score.Clear();
                return;
            }

            Score.Add("In Progress Modifier", (timeLeft - timeToReach) * config.InProgressFateModifier);
        }
    }


    public Vector3 GetDestination()
    {
        return Position.GetPointFromPlayer(Radius);
    }

    public bool NeedSync()
    {
        return MaxLevel < Player.Level && !Player.IsLevelSynced;
    }

    public bool IsInPreparation()
    {
        return State == FateState.Preparation;
    }

    public bool IsSelected()
    {
        return this == FateHelper.SelectedFate;
    }

    public bool IsCurrent()
    {
        return this == FateHelper.CurrentFate;
    }

    public bool IsBlacklisted(Plugin plugin)
    {
        return plugin.Config.FateBlacklist.Contains(Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Fate);
    }

    public bool Equals(Fate? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Fate? left, Fate? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Fate? left, Fate? right)
    {
        return !Equals(left, right);
    }
}
