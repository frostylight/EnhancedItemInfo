using EnhancedItemInfo.Attributes;
using System;

namespace EnhancedItemInfo.Utils;

internal static class AtrtributeUtils {
    public static bool NeedSetup(this Type type) => type.IsDefined(typeof(NeedSetupAttribute), false);
    public static bool IsPatch(this Type type) => type.IsDefined(typeof(PatchAttribute), false);
}
