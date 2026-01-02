using Duckov.UI;
using EnhancedItemInfo.Core.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace EnhancedItemInfo.Core.ItemLevel;

[HarmonyPatch(typeof(ItemDisplay), nameof(ItemDisplay.Setup))]
public static class PatchItemDisplaySetup {
    public static event Action<ItemDisplay>? OnItemDisplayReset = null;
    public static event Action<ItemDisplay>? OnItemDisplayShow = null;

    internal static Dictionary<Item, ItemDisplay> ItemDisplayMap = []; // 等待搜索完毕的物品 => 对应ItemDisplay

    internal static void ResetItemDisplayColor(ItemDisplay itemDisplay) {
        itemDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = ColorUtils.transparent;
    }
    internal static void SetItemDisplayColor(ItemDisplay itemDisplay) {
        itemDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = itemDisplay.Target.GetLevelColor();
    }

    internal static void Postfix(ItemDisplay __instance, Item? target) {
        //if (__instance == null) {
        //    return;
        //}
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
