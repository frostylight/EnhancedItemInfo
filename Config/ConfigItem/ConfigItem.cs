using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedItemInfo.Config.ConfigItem;

internal class ConfigItem<T>: IConfigItem {
    public FieldInfo FieldInfo { get; }
    public string Key { get; }
    public string Description { get; }
    public T Value { get => (T)FieldInfo.GetValue(null); set => FieldInfo.SetValue(null, value); }
    public Action<T>? OnValueChanged { get; }

    public void Callback(T value) {
        Value = value;
        OnValueChanged?.Invoke(Value);
    }

    public ConfigItem(FieldInfo field, string key, string description) {
        if (string.IsNullOrEmpty(key)) {
            throw new ArgumentException($"Key {key} must not be empty");
        }
        if (!field.IsStatic) {
            throw new ArgumentException($"{field.Name} is not static");
        }
        if (field.FieldType != typeof(T)) {
            throw new ArgumentException($"{field.FieldType} is not {typeof(T)}");
        }
        FieldInfo = field;
        Key = key;
        Description = description;
        OnValueChanged = null;
    }
    public ConfigItem(FieldInfo field, string key, string description, Type type, string onValueChanged) : this(field, key, description) {
        if (string.IsNullOrEmpty(onValueChanged)) {
            return;
        }
        var method = AccessTools.DeclaredMethod(type, onValueChanged) ?? throw new MissingMethodException(type.FullName, onValueChanged);
        OnValueChanged = (Action<T>)Delegate.CreateDelegate(type, method);
    }

    public Type ValueType => typeof(T);
    public object? GetValue() => Value;
    public void SetValue(object? value) => Value = (T)value!;
    public void CallbackObject(object? value) => Callback((T)value!);
}
