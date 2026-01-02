using HarmonyLib;
using System;

namespace EnhancedItemInfo.Core.RegisteredMark;

[HarmonyPatch(typeof(ItemAmountDisplay), nameof(ItemAmountDisplay.Setup))]
public class PatchItemAmountDisplaySetup {
    public static event Action<ItemAmountDisplay>? OnItemAmountDisplayShow = null;

    internal static void Postfix(ItemAmountDisplay __instance) {
        OnItemAmountDisplayShow?.Invoke(__instance);
    }
}
