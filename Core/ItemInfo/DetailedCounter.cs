using System.Collections.Generic;

namespace EnhancedItemInfo.Core.ItemInfo;

public class DetailedCounter(string prefix = "") {
    public static string JoinIf(string prefix, params (string, long)[] parts) {
        List<string> lst = new(parts.Length);
        foreach (var (name, value) in parts) {
            if (value != 0) {
                lst.Add($"{name} {value}");
            }
        }
        return $"{prefix} = {string.Join(" + ", lst)}";
    }

    string prefix = prefix;
    readonly List<string> parts = [];

    public int ValidCount => parts.Count;

    public DetailedCounter SetPrefix(string prefix) {
        this.prefix = prefix;
        return this;
    }
    public DetailedCounter AddPart(string name, long value) {
        if (value != 0) {
            parts.Add($"{name} {value}");
        }
        return this;
    }
    public void Clear(string newPrefix = "") {
        prefix = newPrefix;
        parts.Clear();
    }
    public override string ToString() => $"{prefix} = {string.Join(" + ", parts)}";
}
