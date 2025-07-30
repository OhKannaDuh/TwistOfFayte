using Items = TwistOfFayte.Gameplay.Items;

namespace TwistOfFayte.Modules.Currency;

[OcelotModule]
public class CurrencyModule(Plugin plugin, Config config) : Module(plugin, config)
{
    public readonly ItemTracker BicolorGemstones = new(Items.BicolorGemstones);

    public override void Update(UpdateContext context)
    {
        BicolorGemstones.Update();
    }
}
