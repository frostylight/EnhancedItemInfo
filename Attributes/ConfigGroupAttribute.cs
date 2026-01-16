using System;

namespace EnhancedItemInfo.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
internal sealed class ConfigGroupAttribute(string key, string description = "", float scale = 0.7f, bool open = false): Attribute {
    public string Key { get; } = key;
    public string Description { get; } = description;
    public float Scale { get; } = scale;
    public bool Open { get; } = open;

    public bool Root => !string.IsNullOrEmpty(Description);
}
