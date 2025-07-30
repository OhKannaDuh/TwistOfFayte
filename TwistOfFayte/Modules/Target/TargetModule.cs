using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace TwistOfFayte.Modules.Target;

[OcelotModule]
public class TargetModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public bool ShouldTarget = false;

    private IGameObject? Target {
        get => Svc.Targets.Target;

        set {
            if (Svc.Targets.Target?.EntityId == value?.EntityId)
            {
                return;
            }

            Svc.Targets.Target = value;
        }
    }

    private IEnumerable<IBattleNpc> Enemies {
        get => TargetHelper.Enemies;
    }

    private IEnumerable<IBattleNpc> InCombat {
        get => TargetHelper.InCombat;
    }

    private IEnumerable<IBattleNpc> NotInCombat {
        get => TargetHelper.NotInCombat;
    }

    private IEnumerable<IBattleNpc> ForlornMaidens {
        get => TargetHelper.ForlornMaidens;
    }

    private IEnumerable<IBattleNpc> TheForlorns {
        get => TargetHelper.TheForlorns;
    }

    public override void PostInitialize()
    {
        FateHelper.OnLeaveFate += LeaveFate;
    }

    public override void Update(UpdateContext context)
    {
        if (FateHelper.SelectedFate != null && EzThrottler.Throttle("TargetModule.Update.Scan"))
        {
            TargetHelper.Update(FateHelper.SelectedFate);
        }

        // if (FateHelper.CurrentFate == null || Player.Mounted || !ShouldTarget)
        // {
        //     return;
        // }
        //
        // if (Target?.IsTargetingPlayer() == true && NotInCombat.Any())
        // {
        //     Target = null;
        //     Plugin.Chain.Abort();
        // }
        //
        // Target ??= TheForlorns.FirstOrDefault();
        // Target ??= ForlornMaidens.FirstOrDefault();
        // if (Target != null)
        // {
        //     return;
        // }
        //
        // if (!NotInCombat.Any() && InCombat.Any())
        // {
        //     Target = null;
        // }
        //
        // if (!TargetHelper.NotInCombat.Any())
        // {
        //     Target = InCombat.Centroid();
        //     return;
        // }
        //
        // Target ??= NotInCombat.First();
    }


    private void LeaveFate()
    {
        TargetHelper.Clear();
    }

    public override void Dispose()
    {
        FateHelper.OnLeaveFate -= LeaveFate;
    }

    public override void Render(RenderContext context) { }
}
