using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemWeight: ItemInfoUI<ItemWeight> {
    [PlacementConfig("ItemWeight", "EnhancedItemInfo_Config_ItemWeight")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemWeight Instance { get => field ??= new(); } = null;

    protected override bool Setup(Item item) {
        SetText($"{Localizations.EnhancedItemInfo_TotalWeight} {item.TotalWeight:0.##}kg");
        if (item.Slots != null && item.Slots.Count > 0) {
            AppendText($"\t{Localizations.EnhancedItemInfo_SelfWeight} {item.SelfWeight:0.##}kg");
        }
        return true;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        Item prefab = ItemAssetsCollection.GetPrefab(itemMetaData.id);
        SetText($"{Localizations.EnhancedItemInfo_UnitWeight} {prefab.UnitSelfWeight:0.##}kg");
        return true;
    }
}
