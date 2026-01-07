using Duckov.ItemUsage;
using EnhancedItemInfo.Attributes;
using ItemStatsSystem;
using System.Text;
using UnityEngine;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemDurability: ItemInfoUI<ItemDurability> {
    [ToggleConfig("ItemDurability", "物品耐久")]
    public static bool EnableItemDurability = true;
    [ToggleConfig("ItemAvailableCount", "物品可用次数")]
    public static bool EnableItemAvailableCount = true;
    protected override bool Enable => EnableItemDurability && EnableItemAvailableCount;

    public static ItemDurability Instance { get => field ??= new(); } = null;

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
        bool enable = false;
        StringBuilder stringBuilder = new();
        if (EnableItemDurability && item.UseDurability) {
            enable = true;
            stringBuilder.AppendLine($"耐久 {item.Durability:0.##} / {item.MaxDurabilityWithLoss:0.##}");
        }
        if (EnableItemAvailableCount && item.UsageUtilities) {
            var (dynamicUsage, total) = CountDurabilityUse(item.UsageUtilities);
            if (total > 1e-8) {
                enable = true;
                int maxUseCount = Mathf.CeilToInt(item.MaxDurability / total);
                int useCount = Mathf.CeilToInt(item.Durability / total);
                if (dynamicUsage) {
                    stringBuilder.Append("预估");
                }
                stringBuilder.Append($"可用次数 {useCount} / {maxUseCount}");
            }
        }
        if (enable) {
            SetText(stringBuilder.ToString());
        }
        return enable;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return false;
    }
}
