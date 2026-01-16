using System;

namespace EnhancedItemInfo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class PlacementConfigAttribute(string key, string description = "", string onChanged = ""): Attribute {
    public readonly string key = key;
    public readonly string description = description;
    public readonly string onChangedName = onChanged;
}