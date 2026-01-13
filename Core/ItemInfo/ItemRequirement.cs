using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Linq;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemRequirement: ItemInfoUI<ItemRequirement> {
    [ToggleConfig("ItemRequirement", "物品需求")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemRequirement Instance { get => field ??= new(); } = null;

    DetailedCounter Counter { get => field ??= new(); } = null;

    bool Setup(int typeID) {
        var questList = ItemUtils.GetQuestRequirement(typeID);
        var buildingList = ItemUtils.GetBuildingRequirement(typeID);
        var perkList = ItemUtils.GetPerkRequirement(typeID);
        int questTotal = questList.Sum(kv => kv.Amount);
        long buildingTotal = buildingList.Sum(kv => kv.Amount);
        long perkTotal = perkList.Sum(kv => kv.Amount);
        long total = questTotal + buildingTotal + perkTotal;
        if (total == 0) {
            return false;
        }
        Counter.Clear();
        Counter.SetPrefix($"需求 {total}")
            .AddPart("任务", questTotal)
            .AddPart("强化", perkTotal)
            .AddPart("建筑", buildingTotal);
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(Counter.ToString());
        int questCount = 0;
        foreach (var (Name, count) in questList) {
            if (questCount >= 5) { // TODO 配置
                stringBuilder.AppendLine($"<indent=1em>...</indent>");
                break;
            }
            stringBuilder.AppendLine($"<indent=1em>x{count} 任务.{Name}</indent>");
            questCount++;
        }
        int perkCount = 0;
        foreach (var (Name, Amount) in perkList) {
            if (perkCount >= 5) { // TODO 配置
                stringBuilder.AppendLine($"<indent=1em>...</indent>");
                break;
            }
            stringBuilder.AppendLine($"<indent=1em>x{Amount} {Name}</indent>");
            perkCount++;
        }
        int buildingCount = 0;
        foreach (var (Name, Amount) in buildingList) {
            if (buildingCount >= 5) { // TODO 配置
                stringBuilder.AppendLine($"<indent=1em>...</indent>");
                break;
            }
            stringBuilder.AppendLine($"<indent=1em>x{Amount} 建筑.{Name}</indent>");
            buildingCount++;
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
