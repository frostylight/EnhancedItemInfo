using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemCount: ItemInfoUI<ItemCount> {
    [ToggleConfig("ItemCount", "物品已有数量")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemCount Instance { get => field ??= new(); } = null;

    DetailedCounter Counter { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        (int inStorage, int onPlayer, int onPet) = ItemUtils.GetItemCount(typeID);
        int total = inStorage + onPlayer + onPet;
        if (total == 0) {
            return false;
        }
        Counter.Clear();
        Counter.SetPrefix($"已有 {total}")
            .AddPart("仓库", inStorage)
            .AddPart("背包", onPlayer)
            .AddPart("宠物", onPet);
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
