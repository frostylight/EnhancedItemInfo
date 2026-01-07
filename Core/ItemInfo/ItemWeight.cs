using EnhancedItemInfo.Attributes;
using ItemStatsSystem;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class ItemWeight: ItemInfoUI<ItemWeight> {
    [ToggleConfig("ItemWeight", "物品重量")]
    public static bool enable = true;
    protected override bool Enable => enable;

    public static ItemWeight Instance { get => field ??= new(); } = null;

    protected override bool Setup(Item item) {
        SetText($"总重 {item.TotalWeight:0.##}kg");
        if (item.Slots != null && item.Slots.Count > 0) {
            AppendText($"\t自重 {item.SelfWeight:0.##}kg");
        }
        return true;
    }
    protected override bool Setup(ItemMetaData itemMetaData) {
        Item prefab = ItemAssetsCollection.GetPrefab(itemMetaData.id);
        SetText($"单位重量 {prefab.UnitSelfWeight:0.##}kg");
        return true;
    }
}
