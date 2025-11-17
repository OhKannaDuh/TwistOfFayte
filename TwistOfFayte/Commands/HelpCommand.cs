using System.Collections.Generic;
using Ocelot.Services.Commands;
using Ocelot.Services.Translation;
using TwistOfFayte.Windows;

namespace TwistOfFayte.Commands;

public class HelpCommand(HelpWindow window, ITranslator<HelpCommand> translator) : OcelotCommand(translator)
{
    public override string Command { get; } = "help";

    public override List<string> Aliases { get; } = ["h"];

    public override void Execute(CommandContext context)
    {
        window.Toggle();
    }
}
