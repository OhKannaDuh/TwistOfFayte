using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ocelot.Services.Logger;
using Ocelot.Services.Translation;
using Ocelot.Windows;
using OcelotConfigCommand = Ocelot.Services.Commands.ConfigCommand;

namespace TwistOfFayte.Commands;

public class ConfigCommand : OcelotConfigCommand
{
    public override sealed List<string> Aliases
    {
        get => base.Aliases;
    }

    public ConfigCommand(
        IDalamudPluginInterface plugin,
        IConfigWindow window,
        IChatGui chat,
        ILogger<ConfigCommand> logger,
        IEnumerable<IPluginConfiguration> pluginConfigurations,
        ITranslator<ConfigCommand> translator
    ) : base(plugin, window, chat, logger, pluginConfigurations, translator)
    {
        Aliases.AddRange(["tofc", "tofcfg", "tofconfig"]);
    }
}
