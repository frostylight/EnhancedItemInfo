using System;

namespace EnhancedItemInfo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class SliderConfigAttribute(string key, string description = "", float minValue = 0f, float maxValue = 1f, string onChanged = ""): Attribute {
    public readonly string key = key;
    public readonly string description = description;
    public readonly float minValue = minValue;
    public readonly float maxValue = maxValue;
    public readonly string onChangedName = onChanged;
}
