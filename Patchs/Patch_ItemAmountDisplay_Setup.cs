using EnhancedItemInfo.Attributes;
using HarmonyLib;
using System;

namespace EnhancedItemInfo.Patchs;

[Patch]
[HarmonyPatch(typeof(ItemAmountDisplay), nameof(ItemAmountDisplay.Setup))]
public class Patch_ItemAmountDisplay_Setup {
    public static event Action<ItemAmountDisplay>? OnItemAmountDisplayShow = null;

    internal static void Postfix(ItemAmountDisplay __instance) {
        OnItemAmountDisplayShow?.Invoke(__instance);
    }
}
