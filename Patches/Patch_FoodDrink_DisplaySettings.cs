using Duckov.ItemUsage;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Core;
using HarmonyLib;
using SodaCraft.Localizations;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(FoodDrink), nameof(FoodDrink.DisplaySettings), MethodType.Getter)]
internal class Patch_FoodDrink_DisplaySettings {
    static void Postfix(FoodDrink __instance, ref FoodDrink.DisplaySettingsData __result) {
        if (__instance.UseDurability == 0) {
            return;
        }
        __result.description += $" ({Constant.durabilityUsageDescriptionKey.ToPlainText()} : {__instance.UseDurability:0.##})";
    }
}
