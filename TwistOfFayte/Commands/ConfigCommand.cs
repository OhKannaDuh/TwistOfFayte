using System.Collections.Generic;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ocelot.Extensions;
using Ocelot.Services.Translation;
using Ocelot.Windows;
using TwistOfFayte.Config;
using OcelotConfigCommand = Ocelot.Services.Commands.ConfigCommand;

namespace TwistOfFayte.Commands;

public class ConfigCommand : OcelotConfigCommand
{
    public ConfigCommand(
        IDalamudPluginInterface plugin,
        IConfigWindow window,
        IChatGui chat,
        IPluginLog logger,
        ITranslator translator,
        IConfiguration config
    ) : base(plugin, window, chat, logger, translator)
    {
        Aliases.AddRange(["tofc", "tofcfg", "tofconfig"]);
        
        Expose(config);
    }
}
