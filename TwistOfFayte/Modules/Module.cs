using TwistOfFayte.Modules.Currency;
using TwistOfFayte.Modules.Selector;
using TwistOfFayte.Modules.State;
using TwistOfFayte.Modules.Target;
using TwistOfFayte.Modules.Tracker;

namespace TwistOfFayte.Modules;

public class Module(Plugin plugin, Config config) : Module<Plugin, Config>(plugin, config)
{
    [InjectModule]
    public SelectorModule SelectorModule { get; protected set; } = null!;

    [InjectModule]
    public StateModule StateModule { get; protected set; } = null!;

    [InjectModule]
    public TargetModule TargetModule { get; protected set; } = null!;

    [InjectModule]
    public TrackerModule TrackerModule { get; protected set; } = null!;

    [InjectModule]
    public CurrencyModule CurrencyModule { get; protected set; } = null!;

    [InjectIpc(Required = true)]
    public VNavmesh VNavmesh { get; protected set; } = null!;
}
