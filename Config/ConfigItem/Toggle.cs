using EnhancedItemInfo.Attributes;
using System;
using System.Reflection;

namespace EnhancedItemInfo.Config.ConfigItem;

internal class Toggle: ConfigItem<bool> {
    public Toggle(Type type, FieldInfo field, ToggleConfigAttribute attr)
        : base(field, attr.key, attr.description, type, attr.onChangedName) { }
    public Toggle(FieldInfo field, string key, string description)
        : base(field, key, description) { }
    public Toggle(FieldInfo field, string key, string description, Type type, string onValueChanged)
        : base(field, key, description, type, onValueChanged) { }
}
