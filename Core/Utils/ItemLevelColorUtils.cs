using Duckov.Utilities;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedItemInfo.Core.Utils;

public static class ItemLevelColorUtils {
    /// <summary>
    /// 每种物品稀有度颜色的缓存
    /// </summary>
    public static readonly Dictionary<int, Color> itemColorCache = [];

    /// <summary>
    /// [EnhancedItemInfo] 获取Item的稀有度颜色
    /// </summary>
    public static Color GetLevelColor(this Item? item) {
        if (item == null) {
            return ColorUtils.transparent;
        }
        if (itemColorCache.TryGetValue(item.TypeID, out var color)) {
            return color;
        }
        color = GetColorByItem(item);
        itemColorCache.Add(item.TypeID, color);
        return color;
    }
    /// <summary>
    /// [EnhancedItemInfo] 获取ItemMetaData的稀有度颜色
    /// </summary>
    public static Color GetLevelColor(this ItemMetaData data) {
        if (itemColorCache.TryGetValue(data.id, out var color)) {
            return color;
        }
        color = GetColorByItemMetaData(data);
        itemColorCache.Add(data.id, color);
        return color;
    }
    /// <summary>
    /// 获取特定ID物品的稀有度颜色
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static Color GetItemLevelColor(int typeID) {
        if (itemColorCache.TryGetValue(typeID, out var color)) {
            return color;
        }
        color = GetColorByItemMetaData(ItemAssetsCollection.GetMetaData(typeID));
        itemColorCache.Add(typeID, color);
        return color;
    }

    /// <summary>
    /// [EnhancedItemInfo] 根据DisplayQuality获得颜色
    /// </summary>
    /// <returns>
    /// None返回透明，Q7、Q8返回红色
    /// </returns>
    public static Color GetDisplayColor(this DisplayQuality displayQuality) => displayQuality switch {
        DisplayQuality.None => ColorUtils.transparent,
        DisplayQuality.White => ColorUtils.white,
        DisplayQuality.Green => ColorUtils.green,
        DisplayQuality.Blue => ColorUtils.blue,
        DisplayQuality.Purple => ColorUtils.purple,
        DisplayQuality.Orange => ColorUtils.orange,
        DisplayQuality.Red => ColorUtils.red,
        DisplayQuality.Q7 => ColorUtils.red,
        DisplayQuality.Q8 => ColorUtils.red,
        _ => ColorUtils.transparent,
    };

    /// <summary>
    /// 根据物品价值获得颜色
    /// </summary>
    /// <param name="value">物品出售价值（子弹按30堆叠）</param>
    public static Color GetColorByValue(int value) => value switch {
        >= 10000 => ColorUtils.red,
        >= 5000 => ColorUtils.lightRed,
        >= 2500 => ColorUtils.orange,
        >= 1200 => ColorUtils.purple,
        >= 600 => ColorUtils.blue,
        >= 200 => ColorUtils.green,
        // >= 100 => ColorUtils.white,
        _ => ColorUtils.transparent,
    };

    static Color GetModItemColor(DisplayQuality displayQuality, int quality) {
        if (displayQuality != DisplayQuality.None) {
            return displayQuality.GetDisplayColor();
        }
        return ColorUtils.GetColorByLevel(quality);
    }
    static Color GetBulletColor(DisplayQuality displayQuality, int quality, int value) {
        if (displayQuality != DisplayQuality.None) {
            if (displayQuality == DisplayQuality.Orange) {
                return ColorUtils.lightRed;
            }
            return displayQuality.GetDisplayColor();
        }
        Color bulletColor = quality switch {
            1 => ColorUtils.white,
            2 => ColorUtils.green,
            _ => GetColorByValue(value * 15)
        };
        if (bulletColor.GetLevel() > ColorUtils.orange.GetLevel()) {
            bulletColor = ColorUtils.orange;
        }
        return bulletColor;
    }
    static Color GetAccessoryColor(DisplayQuality displayQuality, int quality) {
        if (quality == 999) {
            // 部分分四级的配件 DisplayQuality: None -> Blue -> Purple -> Orange
            if (displayQuality == DisplayQuality.None) { // None 改成 Green 保持稀有度连续
                return ColorUtils.green;
            }
            return displayQuality.GetDisplayColor();
        }
        // 2=>Green 3=>Blue 4=>Purple
        return ColorUtils.GetColorByLevel(quality);
    }
    static Color GetColorFallback(DisplayQuality displayQuality, int value) {
        Color valueColor = GetColorByValue(value / 2);
        Color displayColor = displayQuality.GetDisplayColor();
        if (displayColor.GetLevel() > valueColor.GetLevel()) {
            return displayColor;
        }
        return valueColor;
    }
    static Color GetColorByItem(Item item) {
        if (item.TypeID == 862 || item.TypeID == 1238) { // 特判带火AK、MF毒液
            return ColorUtils.orange;
        }

        DisplayQuality displayQuality = item.DisplayQuality;
        int quality = item.Quality;

        if (ItemAssetsCollection.TryGetDynamicEntry(item.TypeID, out _)) { // 额外新增物品
            return GetModItemColor(displayQuality, quality);
        }

        TagCollection? tags = item.Tags;
        if (tags != null) {
            if (tags.Contains("Bullet")) { // 子弹
                return GetBulletColor(displayQuality, quality, item.Value);
            }
            if (tags.Contains("Accessory")) { // 配件
                return GetAccessoryColor(displayQuality, quality);
            }
            if (tags.Contains("Equipment")) {
                if (tags.Contains("Special")) {
                    if (item.name.Contains("StormProtection")) {
                        return ColorUtils.GetColorByLevel(quality);
                    }
                    return ColorUtils.GetColorByLevel(quality - 1);
                }
                if (quality <= 7) {
                    return ColorUtils.GetColorByLevel(quality);
                }
                return GetColorByValue(item.Value / 2);
            }
        }

        return GetColorFallback(displayQuality, item.Value);
    }
    // 和GetColorByItem一致
    static Color GetColorByItemMetaData(ItemMetaData data) {
        if (data.id == 862 || data.id == 1238) {
            return ColorUtils.orange;
        }

        DisplayQuality displayQuality = data.displayQuality;
        int quality = data.quality;

        if (ItemAssetsCollection.TryGetDynamicEntry(data.id, out _)) {
            return GetModItemColor(displayQuality, quality);
        }

        Tag[]? tags = data.tags;
        if (tags != null) {
            if (tags.Any(tag => tag != null && tag.name.Equals("Bullet"))) {
                return GetBulletColor(displayQuality, quality, data.priceEach);
            }
            if (tags.Any(tag => tag != null && tag.name.Equals("Accessory"))) {
                return GetAccessoryColor(displayQuality, quality);
            }
            if (tags.Any(tag => tag != null && tag.name.Equals("Equipment"))) {
                if (tags.Any(tag => tag != null && tag.name.Equals("Special"))) {
                    if (data.Name.Contains("StormProtection")) {
                        return ColorUtils.GetColorByLevel(quality);
                    }
                    return ColorUtils.GetColorByLevel(quality - 1);
                }
                if (quality <= 7) {
                    return ColorUtils.GetColorByLevel(quality);
                }
                return GetColorByValue(data.priceEach / 2);
            }
        }

        return GetColorFallback(displayQuality, data.priceEach);
    }
}
