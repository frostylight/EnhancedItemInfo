using EnhancedItemInfo.Core.Utils;
using ItemStatsSystem;
using System.Collections.Generic;

namespace EnhancedItemInfo.Core.ItemLevel;

[SubModule]
internal static class ItemLevelManager {
    public static void Init() {
        Logger.Info("ItemLevel is registered");
        ModBehaviour.OnSetupSubmodule += OnSetup;
        ModBehaviour.OnDeactivateSubModule += OnDeactivate;
    }

    public static void OnSetup() {
        Logger.Info($"ItemLevel is enabled");

        InteractableLootbox.OnStartLoot += OnStartLoot;
        InteractableLootbox.OnStopLoot += OnStopLoot;
        PatchItemDisplaySetup.OnItemDisplayReset += PatchItemDisplaySetup.ResetItemDisplayColor;
        PatchItemDisplaySetup.OnItemDisplayShow += PatchItemDisplaySetup.SetItemDisplayColor;
        PatchItemMetaDisplaySetup.OnItemMetaDisplayShow += PatchItemMetaDisplaySetup.SetItemMetaDisplayColor;
    }
    public static void OnDeactivate() {
        Logger.Info($"ItemLevel is disabled");

        InteractableLootbox.OnStartLoot -= OnStartLoot;
        InteractableLootbox.OnStopLoot -= OnStopLoot;
        PatchItemDisplaySetup.OnItemDisplayReset -= PatchItemDisplaySetup.ResetItemDisplayColor;
        PatchItemDisplaySetup.OnItemDisplayShow -= PatchItemDisplaySetup.SetItemDisplayColor;
        PatchItemMetaDisplaySetup.OnItemMetaDisplayShow -= PatchItemMetaDisplaySetup.SetItemMetaDisplayColor;
        OnStopLoot(null);
    }

    public static List<Item> ItemInspecting = [];
    public static void OnStartLoot(InteractableLootbox? lootbox) {
        if (lootbox == null) {
            return;
        }
        Inventory? inventory = lootbox.Inventory;
        if (inventory == null || !inventory.NeedInspection || inventory.hasBeenInspectedInLootBox) {
            return;
        }
        inventory.FindAll(item => item != null && !item.Inspected)
            .ForEach(item => {
                item.onInspectionStateChanged += PatchItemDisplaySetup.OnInspectionStateChanged;
                ItemInspecting.Add(item);
            });
    }
    public static void OnStopLoot(InteractableLootbox? lootbox) {
        ItemInspecting.ForEach(item => {
            item.onInspectionStateChanged -= PatchItemDisplaySetup.OnInspectionStateChanged;
            PatchItemDisplaySetup.ItemDisplayMap.Remove(item);
        });
        ItemInspecting.Clear();
        PatchItemDisplaySetup.ItemDisplayMap.Clear();
    }
}
