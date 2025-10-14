using System;
using System.Linq;
using System.Numerics;
using Ocelot.Services.Data;
using Ocelot.Services.PlayerState;
using TwistOfFayte.Config;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.State;
using TwistOfFayte.Services.Zone;

namespace TwistOfFayte.Services.Fates;

public class FateScorer(
    ScorerConfig scorer,
    TraversalConfig traversal,
    IPlayer player,
    IStateManager state,
    IZone zone
) : IFateScorer
{
    public FateScore Score(Fate fate)
    {
        var score = new FateScore();

        var current = state.GetCurrentFate();
        if (current != null && current.Value == fate.Id)
        {
            score.Add("Current", 1024f);
        }

        var typeScore = fate.Type switch
        {
            FateType.Mobs => scorer.MobFateModifier,
            FateType.Boss => scorer.BossFateModifier,
            FateType.Collect => scorer.CollectFateModifier,
            FateType.Defend => scorer.DefendFateModifier,
            FateType.Escort => scorer.EscortFateModifier,
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
        if (!traversal.ShouldTeleport)
        {
            distance = playerDistance;
        }
        
        score.Add("Distance", (2048 - distance) / 25f);
        
        var teleportRequired = aetheryteDistance < playerDistance && traversal.ShouldTeleport;
        if (teleportRequired)
        {
            score.Add("Teleport Time", -(traversal.TimeToTeleport * traversal.CostPerYalm));
        }

        if (fate.IsBonus)
        {
            score.Add("Bonus Modifier", scorer.BonusFateModifier);
        }

        var estimate = fate.ProgressTracker.Estimate();
        if (estimate == null)
        {
            score.Add("Unstarted Modifier", scorer.UnstartedFateModifier);
        }
        else
        {
            var timeLeft = (float)estimate.Time.TotalSeconds;
            var timeToReach = distance / traversal.CostPerYalm;
            if (teleportRequired)
            {
                timeToReach += traversal.TimeToTeleport;
            }

            // Less than about 30 seconds left when we would arrive
            if (timeToReach > timeLeft - scorer.TimeRequiredToConsiderFate)
            {
                score.Clear();
                return score;
            }

            score.Add("In Progress Modifier", (timeLeft - timeToReach) * scorer.InProgressFateModifier);
        }

        return score;
    }
}
