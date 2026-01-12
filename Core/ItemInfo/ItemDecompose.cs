using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemDecompose: ItemInfoUI<ItemDecompose> {
    [ToggleConfig("ItemDecompose", "物品分解信息")]
    public static bool EnableDecomposeTo = true;
    [ToggleConfig("ItemDecomposeFrom", "物品分解来源")]
    public static bool EnableDecomposeFrom = true;

    protected override bool Enable => EnableDecomposeTo || EnableDecomposeFrom;

    public static ItemDecompose Instance { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        bool enable = false;
        StringBuilder stringBuilder = new();
        if (EnableDecomposeTo) {
            var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
            if (decomposeItems.Length > 0) {
                enable = true;
                stringBuilder.Append("分解:\n");
                foreach (var entry in decomposeItems) {
                    var itemMetaData = ItemAssetsCollection.GetMetaData(entry.id);
                    stringBuilder.AppendLine($"<indent=1em>{entry.amount}x {itemMetaData.DisplayName}</indent>");
                }
            }
        }
        if (EnableDecomposeFrom) {
            var set = ItemUtils.GetDecomposeFromItems(typeID);
            if (set.Count > 0) {
                enable = true;
                stringBuilder.Append("从以下物品拆解得到:\n");
                int count = 0;
                foreach (var (fromItem, amount) in set) {
                    if (count >= 5) { // TODO 配置最大显示数量
                        stringBuilder.AppendLine($"<indent=1em>...</indent>");
                        // 太多的省略
                        break;
                    }
                    var itemMetaData = ItemAssetsCollection.GetMetaData(fromItem);
                    stringBuilder.AppendLine($"<indent=1em>x{amount} {itemMetaData.DisplayName}</indent>");
                    ++count;
                }
            }
        }
        if (enable) {
            SetText(stringBuilder.ToString());
            return true;
        }
        return false;
    }
    protected override bool Setup(Item item) {
        return Setup(item.TypeID);
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        return Setup(itemMetaData.id);
    }
}
