using System.Collections.Generic;
using Ocelot.Commands;
using Ocelot.Modules;
using TwistOfFayte.Windows;

namespace TwistOfFayte.Commands;

[OcelotCommand]
public class ConfigCommand(Plugin plugin) : OcelotCommand
{
    protected override string Command {
        get => "/tofconfig";
    }

    protected override string Description {
        get => "Opens the Twist of Fayte ui";
    }

    protected override IReadOnlyList<string> Aliases { get; set; } = [
        "/tofc"
    ];

    public override void Execute(string command, string arguments)
    {
        plugin.Windows.GetWindow<ConfigWindow>().Toggle();
    }
}
