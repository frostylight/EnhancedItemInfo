using UnityEngine;

namespace EnhancedItemInfo.Extensions;

public static class ColorExtension {
    public static Color WithAlpha(this Color color, float alpha) {
        return new(color.r, color.g, color.b, alpha);
    }
    public static Color WithAlpha(this Color color, int alpha) {
        return new(color.r, color.g, color.b, alpha / 255f);
    }
}
