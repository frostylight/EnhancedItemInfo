using Duckov.ItemUsage;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using ItemStatsSystem;
using System.Text;
using UnityEngine;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemAvailableCount: ItemInfoUI {
    [PlacementConfig("ItemAvailableCount", "EnhancedItemInfo_Config_ItemAvailableCount")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemAvailableCount Instance { get => field ??= new(); } = null;

    static (bool dynamicUsage, float total) CountDurabilityUse(UsageUtilities usage) {
        float total = 0f;
        bool dynamicUsage = false;
        foreach (var behavior in usage.behaviors) {
            if (behavior == null) {
                continue;
            }
            switch (behavior) {
                case FoodDrink foodDrink: {
                    total += foodDrink.UseDurability;
                    break;
                }
                case Drug drug: {
                    if (drug.useDurability) {
                        total += drug.durabilityUsage; // 满治疗量消耗耐久
                        dynamicUsage = true;
                    }
                    break;
                }
                case RemoveBuff removeBuff: {
                    if (removeBuff.useDurability) {
                        total += removeBuff.durabilityUsage;
                    }
                    break;
                }
            }
        }
        return (dynamicUsage, total);
    }
    protected override bool Setup(Item item) {
        if (!item.UsageUtilities) {
            return false;
        }
        var (dynamicUsage, total) = CountDurabilityUse(item.UsageUtilities);
        if (total <= 1e-8) {
            return false;
        }
        StringBuilder stringBuilder = new();
        int maxUseCount = Mathf.CeilToInt(item.MaxDurability / total);
        int useCount = Mathf.CeilToInt(item.Durability / total);
        if (dynamicUsage) {
            stringBuilder.Append(Localizations.EnhancedItemInfo_PredictedAvailableCount);
        }
        else {
            stringBuilder.Append(Localizations.EnhancedItemInfo_AvailableCount);
        }
        stringBuilder.Append($" {useCount} / {maxUseCount}");
        SetText(stringBuilder.ToString());
        return true;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return false;
    }
}
