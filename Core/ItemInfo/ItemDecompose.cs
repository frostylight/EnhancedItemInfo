using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemDecompose: ItemInfoUI<ItemDecompose> {
    [ToggleConfig("ItemDecompose", "物品分解信息")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemDecompose Instance { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
        if (decomposeItems.Length == 0) {
            return false;
        }
        StringBuilder stringBuilder = new("分解:\n", decomposeItems.Length + 1);
        foreach (var entry in decomposeItems) {
            var itemMetaData = ItemAssetsCollection.GetMetaData(entry.id);
            stringBuilder.AppendLine($"<indent=1em>{entry.amount}x {itemMetaData.DisplayName}</indent>");
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
