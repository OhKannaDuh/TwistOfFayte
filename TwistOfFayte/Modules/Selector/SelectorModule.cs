using System.Linq;
using ECommons.DalamudServices;
using TwistOfFayte.Zone;

namespace TwistOfFayte.Modules.Selector;

[OcelotModule]
public class SelectorModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public override SelectorConfig Config {
        get => PluginConfig.SelectorConfig;
    }

    public override void PostInitialize()
    {
        ZoneHelper.GetAetherytes();
    }

    public override void Update(UpdateContext context)
    {
        UpdateCurrentFate(context);
        UpdateSelectedFate(context);
    }

    private void UpdateCurrentFate(UpdateContext context)
    {
        if (!FateHelper.IsInFate() || FateHelper.CurrentFate?.IsActive == false)
        {
            if (FateHelper.CurrentFate != null)
            {
                FateHelper.LeaveFate();
            }

            FateHelper.SetCurrentFate(null);
            return;
        }

        FateHelper.SetCurrentFate(Fate.Current());
    }

    private void UpdateSelectedFate(UpdateContext context)
    {
        if (FateHelper.SelectedFate is { IsActive: true } && StateModule.StateMachine.State != State.State.Idle)
        {
            return;
        }

        var fate = TrackerModule.Fates.Values.FirstOrDefault(f => !f.IsBlacklisted(plugin));
        if (fate != null && FateHelper.SelectedFate != fate)
        {
            Svc.Log.Debug($"Selecting new fate '{fate.Name}', Score: {fate.Score}");

            foreach (var other in TrackerModule.Fates.Values.Where(f => f.Id != fate.Id))
            {
                Svc.Log.Debug($"Not selected fate '{other.Name}', Score: {other.Score}");
            }
        }

        FateHelper.SetSelectedFate(fate);
    }
}
