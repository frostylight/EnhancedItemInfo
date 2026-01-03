using HarmonyLib;
using ItemStatsSystem;
using System;

namespace EnhancedItemInfo.Patchs;

[HarmonyPatch(typeof(ItemMetaDisplay), nameof(ItemMetaDisplay.Setup), [typeof(ItemMetaData)])]
public static class Patch_ItemMetaDisplay_Setup {
    public static event Action<ItemMetaDisplay>? OnItemMetaDisplayShow = null;

    internal static void Postfix(ItemMetaDisplay __instance) {
        OnItemMetaDisplayShow?.Invoke(__instance);
    }
}
