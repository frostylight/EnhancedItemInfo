using Duckov.UI;
using EnhancedItemInfo.Patchs;
using EnhancedItemInfo.Utils;
using UnityEngine.UI;

namespace EnhancedItemInfo.Core.ItemLevel;

[SubModule]
internal static class ItemLevelManager {
    public static void Init() {
        Logger.Info($"{nameof(ItemLevelManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    public static void OnSetup() {
        Logger.Info($"{nameof(ItemLevelManager)} is enabled");

        Patch_ItemDisplay_Setup.OnItemDisplayReset += OnItemDisplayReset;
        Patch_ItemDisplay_Setup.OnItemDisplayShow += OnItemDisplayShow;
        Patch_ItemMetaDisplay_Setup.OnItemMetaDisplayShow += OnItemMetaDisplayShow;
    }
    public static void OnDeactivate() {
        Logger.Info($"{nameof(ItemLevelManager)} is disabled");

        Patch_ItemDisplay_Setup.OnItemDisplayReset -= OnItemDisplayReset;
        Patch_ItemDisplay_Setup.OnItemDisplayShow -= OnItemDisplayShow;
        Patch_ItemMetaDisplay_Setup.OnItemMetaDisplayShow -= OnItemMetaDisplayShow;
    }

    internal static void OnItemDisplayReset(ItemDisplay itemDisplay) {
        itemDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = ColorUtils.transparent;
    }
    internal static void OnItemDisplayShow(ItemDisplay itemDisplay) {
        itemDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = itemDisplay.Target.GetLevelColor();
    }
    internal static void OnItemMetaDisplayShow(ItemMetaDisplay itemMetaDisplay) {
        itemMetaDisplay.transform?.Find("BG")?.GetComponent<Image>()?.color = itemMetaDisplay.GetMetaData().GetLevelColor();
    }
}
