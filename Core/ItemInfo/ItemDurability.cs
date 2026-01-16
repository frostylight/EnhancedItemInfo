using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemDurability: ItemInfoUI<ItemDurability> {
    [PlacementConfig("ItemDurability", "EnhancedItemInfo_Config_ItemDurability")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemDurability Instance { get => field ??= new(); } = null;
    protected override bool Setup(Item item) {
        if (!item.UseDurability) {
            return false;
        }
        SetText($"{Localizations.EnhancedItemInfo_Durability} {item.Durability:0.##} / {item.MaxDurabilityWithLoss:0.##}");
        return true;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return false;
    }
}
