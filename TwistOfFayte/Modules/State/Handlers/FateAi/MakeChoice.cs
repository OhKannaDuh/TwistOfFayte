﻿using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.GameHelpers;
using Ocelot.Extensions;
using Ocelot.States;

namespace TwistOfFayte.Modules.State.Handlers.FateAi;

[State<FateAiState>(FateAiState.MakeChoice)]
public class MakeChoice(StateModule module, FateAiStateMachine stateMachine) : Handler(module, stateMachine)
{
    public override FateAiState? Handle()
    {
        var choices = GetStateScores().OrderByDescending(kv => kv.Value).ToList();
        foreach (var choice in choices)
        {
            module.Debug($" - {choice.Key}: {choice.Value}");
        }

        return choices.First().Key;
    }

    private Dictionary<FateAiState, float> GetStateScores()
    {
        return new Dictionary<FateAiState, float> {
            { FateAiState.GatherMobs, GetGatherMobsScore() },
            { FateAiState.FightGatheredMobs, GetFightGatheredMobsScore() },
            { FateAiState.RepositionMobs, GetRepositionMobsScore() },
            { FateAiState.MaintainFateZone, GetMaintainFateZoneScore() },
        };
    }

    private float GetGatherMobsScore()
    {
        var notInCombat = TargetHelper.NotInCombat.ToList();

        if (notInCombat.Count == 0)
        {
            return 0f;
        }

        var max = module.PluginConfig.GeneralConfig.MaxMobsToFight;
        if (max == 0)
        {
            max = int.MaxValue;
        }

        return TargetHelper.InCombat.Count() < max ? 100f : 0f;
    }

    private float GetFightGatheredMobsScore()
    {
        var inCombat = TargetHelper.InCombat.ToList();

        if (inCombat.Count == 0)
        {
            return 0f;
        }

        if (!TargetHelper.NotInCombat.Any())
        {
            return 100f;
        }

        var max = module.PluginConfig.GeneralConfig.MaxMobsToFight;
        if (max == 0)
        {
            max = int.MaxValue;
        }

        return TargetHelper.InCombat.Count() >= max ? 100f : 0f;
    }

    private float GetRepositionMobsScore()
    {
        return TargetHelper.InCombatOutOfRange.Count() * 10f;
    }

    private float GetMaintainFateZoneScore()
    {
        var fate = FateHelper.CurrentFate;
        if (fate == null)
        {
            return float.MaxValue;
        }

        var distance = Player.Position.Distance2D(fate.Position);
        var radius = fate.Radius;

        if (distance > radius)
        {
            return 1000f + (distance - radius) * 10f;
        }

        var normalized = distance / radius;
        return normalized * 100f;
    }

    private static float AngleDifference(float a, float b)
    {
        var diff = MathF.Abs(a - b) % (MathF.PI * 2);
        return diff > MathF.PI ? MathF.PI * 2 - diff : diff;
    }
}
