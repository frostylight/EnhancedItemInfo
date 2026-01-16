using EnhancedItemInfo.Localization;

namespace EnhancedItemInfo.Extensions;

public static class StringExtension {
    public static string ToLocalization(this string key) {
        return LocalizationManager.GetLocalization(key);
    }
}
