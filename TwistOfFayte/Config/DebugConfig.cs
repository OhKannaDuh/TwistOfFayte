using System;
using Ocelot.Config;
using Ocelot.Config.Fields;

namespace TwistOfFayte.Config;

[Serializable]
[ConfigGroup("other")]
public class DebugConfig : IAutoConfig
{
    [Checkbox] public bool ShowDebug { get; set; } = false;

    [Checkbox] public bool ShouldDrawLines { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForStartNpc { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForTurnInNpc { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForEnemiesTargetingLocalPlayer { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForEnemiesTargetingNoPlayers { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForEnemiesTargetingAnotherPlayerWithoutTankStance { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForEnemiesTargetingAnotherPlayerWithTankStance { get; set; } = false;

    [Checkbox] public bool ShouldShowDebugForEnemyStartTethering { get; set; } = false;
}
