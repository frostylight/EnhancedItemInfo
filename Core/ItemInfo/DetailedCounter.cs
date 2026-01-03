using System.Collections.Generic;

namespace EnhancedItemInfo.Core.ItemInfo;

public static class DetailedCounter {
    public static string JoinIf(string prefix, params (string, long)[] parts) {
        List<string> lst = new(parts.Length);
        foreach (var (name, value) in parts) {
            if (value != 0) {
                lst.Add($"{name} {value}");
            }
        }
        return $"{prefix} = {string.Join(" + ", lst)}";
    }
}
