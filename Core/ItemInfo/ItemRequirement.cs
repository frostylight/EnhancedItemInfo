using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Localization;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using System.Linq;
using System.Text;

namespace EnhancedItemInfo.Core.ItemInfo;

[ConfigGroup("ItemInfo")]
internal class ItemRequirement: ItemInfoUI<ItemRequirement> {
    [PlacementConfig("ItemRequirement", "EnhancedItemInfo_Config_ItemRequirement")]
    public static FeaturePlacement placement = FeaturePlacement.Main;
    protected override bool Enable => placement != FeaturePlacement.None;
    protected override bool Side => placement == FeaturePlacement.Side;

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
        Counter.SetPrefix($"{Localizations.EnhancedItemInfo_Require} {total}")
            .AddPart(Localizations.UI_Quest, questTotal)
            .AddPart(Localizations.EnhancedItemInfo_Perk, perkTotal)
            .AddPart(Localizations.EnhancedItemInfo_Building, buildingTotal);
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(Counter.ToString());
        int questCount = 0;
        foreach (var (Name, count) in questList) {
            if (questCount >= 5) { // TODO 配置
                stringBuilder.AppendLine($"<indent=1em>...</indent>");
                break;
            }
            stringBuilder.AppendLine($"<indent=1em>x{count} {Localizations.UI_Quest}.{Name}</indent>");
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
            stringBuilder.AppendLine($"<indent=1em>x{Amount} {Localizations.EnhancedItemInfo_Building}.{Name}</indent>");
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
