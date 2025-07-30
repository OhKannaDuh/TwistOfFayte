using System;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace TwistOfFayte.Modules.General;

public static class MateriaHelper
{
    public static unsafe bool CanExtract()
    {
        if (!TryGetEquipped(out var equipped))
        {
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
            {
                continue;
            }

            // 10000 is 100%
            if (item->SpiritbondOrCollectability >= 10000)
            {
                return true;
            }
        }

        return false;
    }

    public static unsafe void ExtractEquipped()
    {
        if (!TryGetEquipped(out var equipped))
        {
            return;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
            {
                continue;
            }

            if (item->SpiritbondOrCollectability < 10000)
            {
                continue;
            }

            Plugin.Chain.Submit(chain => {
                chain.RunIf(CanExtract);
                chain.Then(_ => {
                    if (!CanExtract())
                    {
                        return true;
                    }

                    if (Svc.Condition[ConditionFlag.Occupied39] || !EzThrottler.Throttle("MateriaExtract.Check", 100))
                    {
                        return false;
                    }

                    var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("Materialize", 1);
                    if (addon == null)
                    {
                        return true;
                    }

                    // Thank you, Pandora's Box for this
                    var values = stackalloc AtkValue[2];
                    values[0] = new AtkValue {
                        Type = ValueType.Int,
                        Int = 2,
                    };
                    values[1] = new AtkValue {
                        Type = ValueType.UInt,
                        UInt = 0,
                    };

                    addon->FireCallback(2, values);

                    return true;
                });


                chain.RunIf(CanExtract);
                chain.WaitForAddonReady("MaterializeDialog");
                chain.Then(_ => {
                    if (!CanExtract())
                    {
                        return true;
                    }

                    if (Svc.Condition[ConditionFlag.Occupied39] || Svc.GameGui.GetAddonByName("Materialize") == IntPtr.Zero)
                    {
                        return false;
                    }

                    var dialog = Svc.GameGui.GetAddonByName("MaterializeDialog", 1);
                    if (dialog == IntPtr.Zero)
                    {
                        return true;
                    }

                    var addon = (AtkUnitBase*)dialog;
                    if (addon == null)
                    {
                        return true;
                    }

                    new AddonMaster.MaterializeDialog(addon).Materialize();

                    return true;
                });

                chain.Wait(100);

                return chain;
            });
        }
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
