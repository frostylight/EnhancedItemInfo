using EnhancedItemInfo.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnhancedItemInfo.Config.ConfigItem;

public enum FeaturePlacement {
    None = 0,
    Main = 1,
    Side = 2,
}

internal class Placement: ConfigItem<FeaturePlacement> {
    public Placement(FieldInfo field, string key, string description)
        : base(field, key, description) { }

    public Placement(FieldInfo field, string key, string description, Type type, string onValueChanged)
        : base(field, key, description, type, onValueChanged) { }

    public Placement(Type type, FieldInfo field, PlacementConfigAttribute attr)
        : this(field, attr.key, attr.description, type, attr.onChangedName) { }

    public void Callback(string value) {
        if (Enum.TryParse<FeaturePlacement>(value, out var result)) {
            Callback(result);
        }
    }

    public static List<string> Options = [.. Enum.GetNames(typeof(FeaturePlacement))];

    public override object GetValue() => Value.ToString();
    public override void SetValue(object value) {
        if (value is FeaturePlacement placement) {
            Value = placement;
        }
        else if (value is bool bvalue) {
            Value = bvalue ? FeaturePlacement.Main : FeaturePlacement.Side;
        }
        else if (Enum.TryParse<FeaturePlacement>(value.ToString(), out var result)) {
            Value = result;
        }
    }
}
