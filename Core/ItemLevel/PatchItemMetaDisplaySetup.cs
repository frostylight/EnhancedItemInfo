using EnhancedItemInfo.Core.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System;
using UnityEngine.UI;

namespace EnhancedItemInfo.Core.ItemLevel;

[HarmonyPatch(typeof(ItemMetaDisplay), nameof(ItemMetaDisplay.Setup), [typeof(ItemMetaData)])]
public static class PatchItemMetaDisplaySetup {
    public static event Action<ItemMetaDisplay>? OnItemMetaDisplayShow = null;

    internal static void SetItemMetaDisplayColor(ItemMetaDisplay itemMetaDisplay) {
        itemMetaDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = itemMetaDisplay.GetMetaData().GetLevelColor();
    }

    internal static void Postfix(ItemMetaDisplay __instance) {
        OnItemMetaDisplayShow?.Invoke(__instance);
    }
}
