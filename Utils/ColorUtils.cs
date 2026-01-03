using System;
using UnityEngine;

namespace EnhancedItemInfo.Utils;

public static class ColorUtils {
    public static Color RGBA(uint r, uint g, uint b, uint a = 255) => new(r / 255f, g / 255f, b / 255f, a / 255f);
    public static Color RGBA(uint rgba) {
        uint r = (rgba & 0xff000000) >> 24;
        uint g = (rgba & 0xff0000) >> 16;
        uint b = (rgba & 0xff00) >> 8;
        uint a = (rgba & 0xff);
        return RGBA(r, g, b, a);
    }
    public static Color RGB(uint r, uint g, uint b) => RGBA(r, g, b);
    public static Color RGB(uint rgb) => RGBA(rgb << 8);

    public static Color transparent = RGBA(0xffffff_00);
    public static Color white = RGBA(0xffffff_40);
    public static Color green = RGBA(0x7cff7c_40);
    public static Color blue = RGBA(0x7cd5ff_40);
    public static Color purple = RGBA(0xd0acff_40);
    public static Color orange = RGBA(0xffdc24_96);
    public static Color lightRed = RGBA(0xff5858_96);
    public static Color red = RGBA(0xbb0000_96);

    public static Color[] colorLevels = [transparent, white, green, blue, purple, orange, lightRed, red];
    public static readonly string[] colorNames = "transparent, white, green, blue, purple, orange, lightRed, red".Split(",");

    /// <summary>
    /// 截取level获得Color
    /// </summary>
    /// <returns>[transparent, white, green, blue, purple, orange, lightRed, red]</returns>
    public static Color GetColorByLevel(int level) => colorLevels[Math.Clamp(level, 0, colorLevels.Length - 1)];

    /// <summary>
    /// [EnhancedItemInfo] 获取Color等级
    /// </summary>
    public static int GetLevel(this Color color) => colorLevels.IndexOf(color);

    public static string GetName(this Color color) {
        int level = color.GetLevel();
        if (level == -1) {
            return color.ToString();
        }
        return $"{colorNames[level]}({color})";
    }

    public static Color WithAlpha(this Color color, float alpha) {
        return new(color.r, color.g, color.b, alpha);
    }
    public static Color WithAlpha(this Color color, int alpha) {
        return new(color.r, color.g, color.b, alpha / 255f);
    }
}
