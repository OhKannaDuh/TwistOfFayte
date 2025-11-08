using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using Ocelot.Chain;
using TwistOfFayte.Chains.Steps;
using TwistOfFayte.Config;
using TwistOfFayte.Services.Repair.Steps;

namespace TwistOfFayte.Services.Repair;

public class RepairService(IChainFactory chains, GeneralConfig config) : IRepairService
{
    public unsafe bool ShouldRepair()
    {
        if (!TryGetEquipped(out var equipped))
        {
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item is null)
            {
                continue;
            }

            if (Convert.ToInt32(Convert.ToDouble(item->Condition) / 30000.0 * 100.0) <= config.AutoRepairThreshold)
            {
                return true;
            }
        }

        return false;
    }

    public IChain Repair()
    {
        var chain = chains.Create("Repairs");

        chain.Then<UnmountStep>();
        chain.Then<RepairStep>();

        return chain;
    }

    private static unsafe bool TryGetEquipped(out InventoryContainer* equipped)
    {
        equipped = null;

        var inventory = InventoryManager.Instance();
        if (inventory == null)
        {
            return false;
        }

        equipped = inventory->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null || !equipped->IsLoaded)
        {
            equipped = null;
            return false;
        }

        return true;
    }
}
