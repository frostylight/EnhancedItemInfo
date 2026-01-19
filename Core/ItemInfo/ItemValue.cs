using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemValue: ItemInfoUI {
    [PlacementConfig("ItemValue", "EnhancedItemInfo_Config_ItemValue")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemValue Instance { get => field ??= new(); } = null;

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
