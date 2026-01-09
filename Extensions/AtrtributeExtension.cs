using EnhancedItemInfo.Attributes;
using System;

namespace EnhancedItemInfo.Extensions;

internal static class AtrtributeExtension {
    public static bool NeedSetup(this Type type) => type.IsDefined(typeof(NeedSetupAttribute), false);
    public static bool IsPatch(this Type type) => type.IsDefined(typeof(PatchAttribute), false);
}
