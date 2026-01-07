using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemRequirement: ItemInfoUI<ItemRequirement> {
    [ToggleConfig("ItemRequirement", "物品需求")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemRequirement Instance { get => field ??= new(); } = null;

    DetailedCounter Counter { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        int quest = ItemUtils.GetQuestRequirement(typeID);
        long building = ItemUtils.GetBuildingRequirement(typeID);
        long perk = ItemUtils.GetPerkRequirement(typeID);
        long total = quest + building + perk;
        if (total == 0) {
            return false;
        }
        Counter.Clear();
        Counter.SetPrefix($"需求 {total}")
            .AddPart("任务", quest)
            .AddPart("强化", perk)
            .AddPart("建筑", building);
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
