using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemDecompose: ItemInfoUI {
    [PlacementConfig("ItemDecompose", "EnhancedItemInfo_Config_ItemDecompose")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemDecompose Instance { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
        if (decomposeItems.Length == 0) {
            return false;
        }
        StringBuilder stringBuilder = new();
        if (decomposeItems.Length > 0) {
            stringBuilder.Append(Localizations.UI_ItemDecompose + ":\n");
            foreach (var entry in decomposeItems) {
                var itemMetaData = ItemAssetsCollection.GetMetaData(entry.id);
                stringBuilder.AppendLine($"<indent=1em>{entry.amount}x {itemMetaData.DisplayName}</indent>");
            }
        }
        SetText(stringBuilder.ToString());
        return true;
    }
    protected override bool Setup(Item item) {
        return Setup(item.TypeID);
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return Setup(itemMetaData.id);
    }
}
