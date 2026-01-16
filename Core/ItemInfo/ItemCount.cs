using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemCount: ItemInfoUI<ItemCount> {
    [PlacementConfig("ItemCount", "EnhancedItemInfo_Config_ItemCount")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemCount Instance { get => field ??= new(); } = null;

    DetailedCounter Counter { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        (int inStorage, int onPlayer, int onPet) = ItemUtils.GetItemCount(typeID);
        int total = inStorage + onPlayer + onPet;
        if (total == 0) {
            return false;
        }
        Counter.Clear();
        Counter.SetPrefix($"{Localizations.EnhancedItemInfo_Having} {total}")
            .AddPart(Localizations.UI_Inventory_Storage, inStorage)
            .AddPart(Localizations.UI_Inventory_Backpack, onPlayer)
            .AddPart(Localizations.UI_LootBox_Safe, onPet);
        SetText(Counter.ToString());
        return true;
    }
    protected override bool Setup(Item item) {
        return Setup(item.TypeID);
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return Setup(itemMetaData.id);
    }
}
