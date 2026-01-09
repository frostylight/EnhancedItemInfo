using EnhancedItemInfo.Core;
using EnhancedItemInfo.Utils.ItemMeta;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedItemInfo.Extensions;

public static class ItemMetaExtension {
    public static IItemMeta GetMeta(this Item item) => new ItemAdapter(item);
    public static IItemMeta GetMeta(this ItemMetaData itemMetaData) => new ItemMetaDataAdapter(itemMetaData);

    public static bool HasTag(this IItemMeta itemMeta, string tag) {
        var tags = itemMeta.Tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(t => t != null && t.name.Equals(tag));
    }
    public static bool HasAnyTag(this IItemMeta itemMeta, IEnumerable<string> tag) {
        var tags = itemMeta.Tags;
        if (tags == null) {
            return false;
        }
        var set = tag.ToHashSet();
        return tags.Any(t => t != null && set.Contains(t.name));
    }
    public static bool IsKey(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemKeyTag);
    }
    public static bool IsFormula(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemFormulaTag);
    }
    public static bool IsBullet(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemBulletTag);
    }
    public static bool IsAccessory(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemAccessoryTag);
    }
    public static bool IsEquipment(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemEquipmentTag);
    }
    public static bool IsSpecial(this IItemMeta itemMeta) {
        return itemMeta.HasTag(Constant.ItemSpecialTag);
    }
}
