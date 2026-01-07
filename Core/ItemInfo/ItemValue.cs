using EnhancedItemInfo.Attributes;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemValue: ItemInfoUI<ItemValue> {
    [ToggleConfig("ItemValue", "物品价值")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemCount Instance { get => field ??= new(); } = null;

    protected override bool Setup(Item item) {
        SetText($"${item.GetTotalRawValue() / 2f:0.##}");
        AppendTextIf(item.Stackable, $" ({item.Value / 2f:0.##})");
        return true;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        SetText($"${itemMetaData.priceEach / 2f:0.##}");
        return true;
    }
}
