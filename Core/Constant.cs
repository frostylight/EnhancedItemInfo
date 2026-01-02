using EnhancedItemInfo.Core.Utils;
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
}
