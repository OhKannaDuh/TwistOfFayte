using System.Collections.Generic;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ocelot.Services.Commands;
using Ocelot.Services.Translation;
using Ocelot.Windows;
using OcelotMainCommand = Ocelot.Services.Commands.MainCommand;

namespace TwistOfFayte.Commands;

public class MainCommand(
    ITranslator translator,
    IDalamudPluginInterface plugin,
    IChatGui chat,
    IMainWindow window,
    IEnumerable<IMainCommandDelegate> delegates
) : OcelotMainCommand(translator, plugin, chat, window, delegates)
{
    public override List<string> Aliases
    {
        get => ["tof"];
    }
}
