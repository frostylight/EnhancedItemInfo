using System.Collections.Generic;

namespace EnhancedItemInfo.Extensions;

public static class DictionaryExtension {
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : new() {
        if (!dict.TryGetValue(key, out TValue value)) {
            value = new();
            dict[key] = value;
        }
        return value;
    }
}
