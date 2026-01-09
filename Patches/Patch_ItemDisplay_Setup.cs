using Duckov.UI;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace EnhancedItemInfo.Patches;

[Patch]
[NeedSetup]
[HarmonyPatch(typeof(ItemDisplay), nameof(ItemDisplay.Setup))]
public static class Patch_ItemDisplay_Setup {
    public static event Action<ItemDisplay>? OnItemDisplayReset = null;
    public static event Action<ItemDisplay>? OnItemDisplayShow = null;

    internal static Dictionary<Item, ItemDisplay> ItemDisplayMap = []; // 等待搜索完毕的物品 => 对应ItemDisplay
    internal static List<Item> ItemInspecting = []; // 正在搜索的物品

    internal static void Init() {
        Logger.Info($"{nameof(Patch_ItemDisplay_Setup)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    internal static void OnSetup() {
        Logger.Info($"{nameof(Patch_ItemDisplay_Setup)} is enabled");

        InteractableLootbox.OnStartLoot += OnStartLoot;
        InteractableLootbox.OnStopLoot += OnStopLoot;
    }
    internal static void OnDeactivate() {
        Logger.Info($"{nameof(Patch_ItemDisplay_Setup)} is disabled");

        InteractableLootbox.OnStartLoot -= OnStartLoot;
        InteractableLootbox.OnStopLoot -= OnStopLoot;
        OnStopLoot(null);
    }

    internal static void Postfix(ItemDisplay __instance, Item? target) {
        Image? background = __instance.transform?.Find("BG")?.GetComponent<Image>();
        if (target == null) {
            OnItemDisplayReset?.Invoke(__instance);
            return;
        }
        if (target.NeedInspection) {
            // 还未搜索结束
            OnItemDisplayReset?.Invoke(__instance);
            ItemDisplayMap.Add(target, __instance);
            return;
        }
        OnItemDisplayShow?.Invoke(__instance);
    }

    internal static void OnStartLoot(InteractableLootbox? lootbox) {
        if (lootbox == null) {
            return;
        }
        Inventory? inventory = lootbox.Inventory;
        if (inventory == null || !inventory.NeedInspection || inventory.hasBeenInspectedInLootBox) {
            return;
        }
        inventory.FindAll(item => item != null && !item.Inspected)
            .ForEach(item => {
                item.onInspectionStateChanged += OnInspectionStateChanged;
                ItemInspecting.Add(item);
            });
    }
    internal static void OnStopLoot(InteractableLootbox? lootbox) {
        ItemInspecting.ForEach(item => {
            item.onInspectionStateChanged -= OnInspectionStateChanged;
            ItemDisplayMap.Remove(item);
        });
        ItemInspecting.Clear();
        ItemDisplayMap.Clear();
    }

    internal static void OnInspectionStateChanged(Item item) {
        if (!item.Inspected) {
            return;
        }
        if (!ItemDisplayMap.TryGetValue(item, out ItemDisplay itemDisplay)) {
            Logger.Warn($"Failed to get ItemDisplay for Item({item.DisplayName})");
            return;
        }
        if (itemDisplay.Target != item) {
            Logger.Warn($"Item({item.DisplayName}) and ItemDisplay mismatch");
            return;
        }
        item.onInspectionStateChanged -= OnInspectionStateChanged;
        ItemDisplayMap.Remove(item);
        OnItemDisplayShow?.Invoke(itemDisplay);
    }
}
