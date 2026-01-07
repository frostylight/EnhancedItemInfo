using Duckov.Modding;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EnhancedItemInfo.Config;

internal static class Setting {
    public static bool Inited { get; private set; } = false;
    public static ModInfo modInfo;
    public static bool ModSettingEnable { get; private set; } = false;

    public static List<IConfigItem> configItems = [];

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
                configItems.Add(new Toggle(type, field, attr));
            }
        }

        Load();
        Inited = true;
    }

    public static void OnSetup() {
        ModSettingEnable = ModSettingAPI.Init();
        if (ModSettingEnable) {
            configItems.ForEach(item => {
                switch (item) {
                    case Toggle toggle: {
                        ModSettingAPI.AddToggle(modInfo, toggle.Key, toggle.Description, toggle.Value, toggle.Callback);
                        break;
                    }
                    // TODO other config
                }
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
        foreach (var item in configItems) {
            if (data.TryGetValue(item.Key, out var valueObject)) {
                if (valueObject.GetType() == item.ValueType) {
                    item.SetValue(valueObject);
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
        foreach (var item in configItems) {
            setting.Add(item.Key, item.GetValue()!);
        }
        var json = JsonConvert.SerializeObject(setting, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
