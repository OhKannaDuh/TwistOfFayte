using ECommons.DalamudServices;
using Ocelot.Commands;
using Ocelot.Modules;
using TwistOfFayte.Windows;

namespace TwistOfFayte.Commands;

[OcelotCommand]
public class MainCommand(Plugin plugin) : OcelotCommand
{
    protected override string Command {
        get => "/tof";
    }

    protected override string Description {
        get => "Opens the Twist of Fayte ui";
    }

    public override void Execute(string command, string arguments)
    {
        if (arguments.Trim() == "config" || arguments.Trim() == "c")
        {
            new ConfigCommand(plugin).Execute(command, arguments);
            return;
        }

        plugin.Windows.GetWindow<MainWindow>().Toggle();
    }
}
