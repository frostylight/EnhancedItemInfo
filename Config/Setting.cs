using Duckov.Modding;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EnhancedItemInfo.Config;

internal static class Setting {
    public static bool Inited { get; private set; } = false;
    public static ModInfo modInfo;
    public static bool ModSettingEnable { get; private set; } = false;

    public sealed class Toggle(FieldInfo fieldInfo, string key, string description, Action<bool>? callback = null) {
        public readonly FieldInfo Field = fieldInfo;
        public readonly string Key = key;
        public readonly string Description = description;
        public readonly Action<bool>? Callback = callback;

        public void OnValueChange(bool value) {
            Field.SetValue(null, value);
            Callback?.Invoke(value);
        }
    }
    public static List<Toggle> toggles = [];

    public static void Init(ModInfo info) {
        modInfo = info;
        ModBehaviour.OnSetup += OnSetup;

        foreach (var type in AccessTools.GetTypesFromAssembly(ModBehaviour.assembly)) {
            if (type == null) {
                continue;
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (field == null) {
                    continue;
                }
                var attr = field.GetCustomAttribute<ToggleConfigAttribute>();
                if (attr == null) {
                    continue;
                }
                if (field.FieldType != typeof(bool)) {
                    Logger.Warn($"Toggle {type.FullName}.{field.Name} is not bool");
                }
                if (attr.onChangedName != "") {
                    try {
                        var method = AccessTools.DeclaredMethod(type, attr.onChangedName, [typeof(bool)]) ?? throw new MissingMethodException(type.FullName, attr.onChangedName);
                        var onChanged = Delegate.CreateDelegate(typeof(Action<bool>), method);
                        toggles.Add(new(field, attr.key, attr.description, (Action<bool>)onChanged));
                    }
                    catch (Exception ex) {
                        Logger.Error($"Failed to create callback for {attr.key}", ex);
                        toggles.Add(new(field, attr.key, attr.description));
                    }
                }
                else {
                    toggles.Add(new(field, attr.key, attr.description));
                }
            }
        }

        Load();
        Inited = true;
    }

    public static void OnSetup() {
        ModSettingEnable = ModSettingAPI.Init();
        if (ModSettingEnable) {
            toggles.ForEach(toggle => {
                bool defaultValue = (bool)toggle.Field.GetValue(null);
                ModSettingAPI.AddToggle(modInfo, toggle.Key, toggle.Description, defaultValue, toggle.OnValueChange);
            });
            return;
        }
    }

    public static void Load() {
        string path = Path.Combine(modInfo.path, "config.json");
        if (!File.Exists(path)) {
            return;
        }
        Logger.Info($"Loading Setting from {path}");
        string json = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        if (data == null) {
            Logger.Warn($"Failed to read config");
            return;
        }
        if (data.TryGetValue("Version", out var versionObject)) {
            if (versionObject is string version) {
                Logger.Info($"Config version {version}");
            }
            else {
                Logger.Warn($"Failed to get config version");
            }
        }
        foreach (var toggle in toggles) {
            if (data.TryGetValue(toggle.Key, out var valueObject)) {
                if (valueObject is bool value) {
                    toggle.Field.SetValue(null, value);
                }
                else {
                    Logger.Warn($"Failed to get toggle {toggle.Key} {toggle.Description}");
                }
            }
        }
    }
    public static void Save() {
        if (!Inited) {
            return;
        }
        string path = Path.Combine(modInfo.path, "config.json");
        Dictionary<string, object> setting = [];
        setting.Add("Version", modInfo.version);
        foreach (var toggle in toggles) {
            var value = toggle.Field.GetValue(null);
            setting.Add(toggle.Key, value);
        }
        var json = JsonConvert.SerializeObject(setting, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
