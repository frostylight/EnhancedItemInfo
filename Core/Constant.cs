using EnhancedItemInfo.Utils;
using UnityEngine;

namespace EnhancedItemInfo.Core;

public static class Constant {
    public const int LRUCacheCapacity = 25;

    public const string RegisteredMarkBackgroundKey = "MarkBackground";
    public const string RegisteredMarkTextKey = "MarkText";
    public static readonly Vector2 MarkPostion = new(-5, -5);
    public static readonly Vector2 MarkSize = new(28, 28);
    public static readonly Color MarkBackgroundColor = Color.white.WithAlpha(0.8f);
    public static readonly Color MarkColor = Color.magenta;

    public const string KeyTag = "Key";
    public const string FormulaTag = "Formula_Blueprint";

    public static Color Transparent = ColorUtils.RGBA(0xffffff_00);
    public static Color White = ColorUtils.RGBA(0xffffff_40);
    public static Color Green = ColorUtils.RGBA(0x7cff7c_40);
    public static Color Blue = ColorUtils.RGBA(0x7cd5ff_40);
    public static Color Purple = ColorUtils.RGBA(0xd0acff_40);
    public static Color Orange = ColorUtils.RGBA(0xffdc24_96);
    public static Color LightRed = ColorUtils.RGBA(0xff5858_96);
    public static Color Red = ColorUtils.RGBA(0xbb0000_96);

    public const string durabilityUsageDescriptionKey = "Usage_Durability";
}
