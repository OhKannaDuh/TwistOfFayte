using System;
using System.Collections.Generic;
using ECommons.DalamudServices;
using TwistOfFayte.Modules.Debug;
using TwistOfFayte.Modules.General;
using TwistOfFayte.Modules.Selector;
using TwistOfFayte.Modules.Ui;

namespace TwistOfFayte;

[Serializable]
public class Config : IOcelotConfig
{
    public int Version { get; set; } = 1;

    public SelectorConfig SelectorConfig { get; set; } = new();

    public GeneralConfig GeneralConfig { get; set; } = new();

    public UiConfig UiConfig { get; set; } = new();

    public DebugConfig DebugConfig { get; set; } = new();

    public List<uint> FateBlacklist { get; set; } = [];

    public void Save()
    {
        Svc.PluginInterface.SavePluginConfig(this);
    }
}
