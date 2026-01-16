using Duckov.ItemUsage;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Localization;
using HarmonyLib;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(FoodDrink), nameof(FoodDrink.DisplaySettings), MethodType.Getter)]
internal class Patch_FoodDrink_DisplaySettings {
    static void Postfix(FoodDrink __instance, ref FoodDrink.DisplaySettingsData __result) {
        if (__instance.UseDurability == 0) {
            return;
        }
        __result.description += $" ({Localizations.Usage_Durability} : {__instance.UseDurability:0.##})";
    }
}
