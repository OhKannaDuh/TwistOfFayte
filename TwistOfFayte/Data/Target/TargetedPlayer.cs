using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using Ocelot.Services.Data;

namespace TwistOfFayte.Data;

public unsafe class TargetedPlayer(BattleChara* player, bool local, IDataRepository<ClassJob> data)
{
    private ClassJob? datum
    {
        get => data.ContainsKey(player->ClassJob) ? data.Get(player->ClassJob) : null;
    }


    public bool IsLocalPlayer()
    {
        return local;
    }

    public bool IsCharacter()
    {
        return player->IsCharacter();
    }

    public bool IsTank()
    {
        var job = (Job)player->ClassJob;
        return job.IsTank();
    }

    public bool HasTankStanceOn()
    {
        var job = (Job)player->ClassJob;
        if (!job.IsTank())
        {
            return false;
        }

        uint id = (uint)job switch
        {
            1 or 19 => 79,
            3 or 21 => 91,
            32 => 743,
            37 => 1833,
            _ => 0,
        };

        return player->StatusManager.HasStatus(id);
    }
}
