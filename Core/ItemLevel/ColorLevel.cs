using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils.ItemMeta;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedItemInfo.Core.ItemLevel;

internal static class ColorLevel {
    public enum Level {
        Transparent = 0,
        White = 1,
        Green = 2,
        Blue = 3,
        Purple = 4,
        Orange = 5,
        LightRed = 6,
        Red = 7
    }

    public static Color ToColor(this Level colorLevel) {
        return colorLevel switch {
            Level.Transparent => Constant.Transparent,
            Level.White => Constant.White,
            Level.Green => Constant.Green,
            Level.Blue => Constant.Blue,
            Level.Purple => Constant.Purple,
            Level.Orange => Constant.Orange,
            Level.LightRed => Constant.LightRed,
            Level.Red => Constant.Red,
            _ => Constant.Transparent,
        };
    }

    /// <summary>
    /// 每种物品稀有度颜色的缓存
    /// </summary>
    public static readonly Dictionary<int, Level> ItemLevelCache = [];
    public static Level GetLevel(this IItemMeta itemMeta) {
        if (ItemLevelCache.TryGetValue(itemMeta.TypeID, out Level level)) {
            return level;
        }
        level = GetLevelWithoutCache(itemMeta);
        ItemLevelCache.Add(itemMeta.TypeID, level);
        return level;
    }
    public static Color GetLevelColor(this IItemMeta itemMeta) {
        return itemMeta.GetLevel().ToColor();
    }

    static Level GetLevelByDisplayQuality(DisplayQuality displayQuality) {
        return displayQuality switch {
            DisplayQuality.None => Level.Transparent,
            DisplayQuality.White => Level.White,
            DisplayQuality.Green => Level.Green,
            DisplayQuality.Blue => Level.Blue,
            DisplayQuality.Purple => Level.Purple,
            DisplayQuality.Orange => Level.Orange,
            DisplayQuality.Red => Level.Red,
            DisplayQuality.Q7 => Level.Red,
            DisplayQuality.Q8 => Level.Red,
            _ => Level.Transparent,
        };
    }
    static Level GetLevelByQuality(int quality) {
        return (Level)Math.Clamp(quality, 0, (int)Level.Red);
    }
    static Level GetLevelBySellPrice(int sellPrice) {
        return sellPrice switch {
            >= 10000 => Level.Red,
            >= 5000 => Level.LightRed,
            >= 2500 => Level.Orange,
            >= 1200 => Level.Purple,
            >= 600 => Level.Blue,
            >= 200 => Level.Green,
            _ => Level.Transparent,
        };
    }

    public static Level GetModItemLevel(IItemMeta itemMeta) {
        var level = GetLevelByDisplayQuality(itemMeta.DisplayQuality);
        if (level != Level.Transparent) {
            return level;
        }
        return GetLevelByQuality(itemMeta.Quality);
    }
    public static Level GetBulletLevel(IItemMeta itemMeta) {
        var displayQuality = itemMeta.DisplayQuality;
        if (displayQuality != DisplayQuality.None) {
            if (displayQuality == DisplayQuality.Orange) {
                return Level.LightRed;
            }
            return GetLevelByDisplayQuality(displayQuality);
        }
        var quality = itemMeta.Quality;
        if (quality >= 1 && quality <= 2) {
            return GetLevelByQuality(quality);
        }
        var level = GetLevelBySellPrice(itemMeta.PriceEach * 15); // (value / 2) *30
        if (level >= Level.Orange) {
            return Level.Orange;
        }
        return level;
    }
    public static Level GetAccessoryLevel(IItemMeta itemMeta) {
        var quality = itemMeta.Quality;
        if (quality != 999) {
            return GetLevelByQuality(quality);
        }
        // 特殊四级配件 None -> Blue -> Purple -> Orange
        var displayQuality = itemMeta.DisplayQuality;
        if (displayQuality == DisplayQuality.None) {
            return Level.Green;
        }
        return GetLevelByDisplayQuality(displayQuality);
    }
    public static Level GetUniversalLevel(IItemMeta itemMeta) {
        var valueLevel = GetLevelBySellPrice(itemMeta.PriceEach / 2);
        var displayQualityLevel = GetLevelByDisplayQuality(itemMeta.DisplayQuality);
        if (valueLevel >= displayQualityLevel) {
            return valueLevel;
        }
        else {
            return displayQualityLevel;
        }
    }

    static Level GetLevelWithoutCache(IItemMeta itemMeta) {
        if (itemMeta.TypeID == 862 || itemMeta.TypeID == 1238) { // 带火AK、MF毒液与AK、MF数据一致，特判
            return Level.Orange;
        }

        if (ItemAssetsCollection.TryGetDynamicEntry(itemMeta.TypeID, out _)) { // Mod增加物品
            return GetModItemLevel(itemMeta);
        }

        if (itemMeta.IsBullet()) {
            return GetBulletLevel(itemMeta);
        }
        if (itemMeta.IsAccessory()) {
            return GetAccessoryLevel(itemMeta);
        }
        if (itemMeta.IsEquipment()) {
            if (itemMeta.IsSpecial()) {
                if (itemMeta.Name.Contains("StormProtection")) {
                    return GetLevelByQuality(itemMeta.Quality);
                }
                return GetLevelByQuality(itemMeta.Quality - 1);
            }
            if (itemMeta.Quality <= 7) {
                return GetLevelByQuality(itemMeta.Quality);
            }
            return GetLevelBySellPrice(itemMeta.PriceEach / 2);
        }

        return GetUniversalLevel(itemMeta);
    }
}
