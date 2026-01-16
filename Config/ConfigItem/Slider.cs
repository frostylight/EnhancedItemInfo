using EnhancedItemInfo.Attributes;
using System;
using System.Reflection;

namespace EnhancedItemInfo.Config.ConfigItem;

internal class Slider: ConfigItem<float> {
    public float MinValue { get; }
    public float MaxValue { get; }

    public Slider(FieldInfo field, string key, string description, float minValue, float maxValue)
        : base(field, key, description) {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    public Slider(FieldInfo field, string key, string description, float minValue, float maxValue, Type type, string onValueChanged)
        : base(field, key, description, type, onValueChanged) {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    public Slider(Type type, FieldInfo field, SliderConfigAttribute attr)
        : base(field, attr.key, attr.description, type, attr.onChangedName) {
        MinValue = attr.minValue;
        MaxValue = attr.maxValue;
    }
}
