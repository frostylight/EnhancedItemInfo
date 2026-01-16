using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemDecomposeFrom: ItemInfoUI<ItemDecompose> {
    [PlacementConfig("ItemDecomposeFrom", "EnhancedItemInfo_Config_ItemDecomposeFrom")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

    public static ItemDecomposeFrom Instance { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        var set = ItemUtils.GetDecomposeFromItems(typeID);
        if (set.Count == 0) {
            return false;
        }
        StringBuilder stringBuilder = new();
        stringBuilder.Append(Localizations.EnhancedItemInfo_ItemDecomposeFrom + ":\n");
        int count = 0;
        foreach (var (fromItem, amount) in set) {
            if (count >= 5) { // TODO 配置最大显示数量
                stringBuilder.AppendLine($"<indent=1em>...</indent>");
                break;
            }
            var itemMetaData = ItemAssetsCollection.GetMetaData(fromItem);
            stringBuilder.AppendLine($"<indent=1em>x{amount} {itemMetaData.DisplayName}</indent>");
            ++count;
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
