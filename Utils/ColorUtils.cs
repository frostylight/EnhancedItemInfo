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
}
