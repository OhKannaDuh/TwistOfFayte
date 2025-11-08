using System;
using System.Linq;
using System.Numerics;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.State;
using TwistOfFayte.Services.Zone;

namespace TwistOfFayte.Services.Fates;

public class FateScorer(
    ScorerConfig scorerConfig,
    TraversalConfig traversalConfig,
    FateSelectorConfig selectorConfig,
    IPlayer player,
    IStateManager state,
    IZone zone
) : IFateScorer
{
    public FateScore Score(Fate fate)
    {
        var score = new FateScore();

        if (!selectorConfig.ShouldDoFate(fate))
        {
            return score;
        }

        var current = state.GetCurrentFate();
        if (current != null && current.Value == fate.Id)
        {
            score.Add("Current", 1024f);
        }

        var typeScore = fate.Type switch
        {
            FateType.Mobs => scorerConfig.MobFateModifier,
            FateType.Boss => scorerConfig.BossFateModifier,
            FateType.Collect => scorerConfig.CollectFateModifier,
            FateType.Defend => scorerConfig.DefendFateModifier,
            FateType.Escort => scorerConfig.EscortFateModifier,
            _ => 0f,
        };

        if (typeScore != 0f)
        {
            score.Add("Type", typeScore);
        }

        var aetheryteDistance = zone.Aetherytes
            .Select(a => Vector3.Distance(fate.Position, a.Position))
            .Order()
            .FirstOrDefault(float.MaxValue);

        var playerDistance = Vector3.Distance(fate.Position, player.GetPosition());

        var distance = Math.Min(aetheryteDistance, playerDistance);
        if (!traversalConfig.ShouldTeleport)
        {
            distance = playerDistance;
        }

        score.Add("Distance", (2048 - distance) / 25f);

        var teleportRequired = aetheryteDistance < playerDistance && traversalConfig.ShouldTeleport;
        if (teleportRequired)
        {
            score.Add("Teleport Time", -(traversalConfig.TimeToTeleport * traversalConfig.CostPerYalm));
        }

        if (fate.IsBonus)
        {
            score.Add("Bonus Modifier", scorerConfig.BonusFateModifier);
        }

        var estimate = fate.ProgressTracker.Estimate();
        if (estimate == null)
        {
            score.Add("Unstarted Modifier", scorerConfig.UnstartedFateModifier);
        }
        else
        {
            var timeLeft = (float)estimate.Time.TotalSeconds;
            var timeToReach = distance / traversalConfig.CostPerYalm;
            if (teleportRequired)
            {
                timeToReach += traversalConfig.TimeToTeleport;
            }

            if (timeToReach > timeLeft - scorerConfig.TimeRequiredToConsiderFate)
            {
                score.Clear();
                return score;
            }

            score.Add("In Progress Modifier", (timeLeft - timeToReach) * scorerConfig.InProgressFateModifier);
        }

        return score;
    }
}
