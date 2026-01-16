using System;
using System.Reflection;

namespace EnhancedItemInfo.Config.ConfigItem;

internal interface IConfigItem {
    public FieldInfo FieldInfo { get; }
    public string Key { get; }
    public string Description { get; }

    public Type ValueType { get; }
    public object GetValue();
    public void SetValue(object value);

    public int GetHashCode() => Key.GetHashCode();
}