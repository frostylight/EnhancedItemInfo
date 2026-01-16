using Duckov.Modding;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Config.ConfigItem;
using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EnhancedItemInfo.Config;

internal static class ConfigManager {
    public static bool Inited { get; private set; } = false;
    public static ModInfo modInfo;
    public static bool ModSettingEnable { get; private set; } = false;

    public static List<IConfigItem> configItems = [];
    public static Dictionary<string, Group> groups = [];

    public static Group GetUpdatedGroup(ConfigGroupAttribute attr) {
        if (groups.TryGetValue(attr.Key, out var group)) {
            if (attr.Root) {
                group.Description = attr.Description;
                group.Scale = attr.Scale;
                group.Open = attr.Open;
            }
        }
        else {
            group = new(attr);
            groups[attr.Key] = group;
        }
        return group;
    }

    public static void Init(ModInfo info) {
        if (Inited) {
            return;
        }
        modInfo = info;
        ModBehaviour.OnSetup += OnSetup;

        foreach (var type in AccessTools.GetTypesFromAssembly(ModBehaviour.assembly)) {
            if (type == null) {
                continue;
            }
            Group? classGroup = null;
            var classGroupAttr = type.GetCustomAttribute<ConfigGroupAttribute>();
            if (classGroupAttr != null) {
                classGroup = GetUpdatedGroup(classGroupAttr);
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (field == null) {
                    continue;
                }
                var group = classGroup;
                var groupAttr = field.GetCustomAttribute<ConfigGroupAttribute>();
                if (groupAttr != null) {
                    group = GetUpdatedGroup(groupAttr);
                }
                foreach (var attr in field.GetCustomAttributes()) {
                    if (attr == null) {
                        continue;
                    }
                    bool flag = false;
                    try {
                        switch (attr) {
                            case ToggleConfigAttribute toggleAttr: {
                                configItems.Add(new Toggle(type, field, toggleAttr));
                                group?.Keys.Add(toggleAttr.key);
                                flag = true;
                                break;
                            }
                            case SliderConfigAttribute sliderAttr: {
                                configItems.Add(new Slider(type, field, sliderAttr));
                                group?.Keys.Add(sliderAttr.key);
                                flag = true;
                                break;
                            }
                            case PlacementConfigAttribute placementAttr: {
                                configItems.Add(new Placement(type, field, placementAttr));
                                group?.Keys.Add(placementAttr.key);
                                flag = true;
                                break;
                            }
                            // TODO other config
                        }
                    }
                    catch (Exception ex) {
                        Logger.Error($"Failed to Parse Config {type.FullName}.{field.Name}", ex);
                        break;
                    }

                    if (flag) {
                        break;
                    }
                }
            }
        }

        Load();
        Inited = true;
    }

    public static void OnSetup() {
        ModSettingEnable = ModSettingAPI.Init(modInfo);
        if (ModSettingEnable) {
            configItems.ForEach(item => {
                switch (item) {
                    case Toggle toggle: {
                        ModSettingAPI.AddToggle(toggle.Key, toggle.Description.ToLocalization(), toggle.Value, toggle.Callback);
                        break;
                    }
                    case Slider slider: {
                        ModSettingAPI.AddSlider(slider.Key, slider.Description.ToLocalization(), slider.Value, new(slider.MinValue, slider.MaxValue), slider.Callback);
                        break;
                    }
                    case Placement placement: {
                        ModSettingAPI.AddDropdownList(placement.Key, placement.Description.ToLocalization(), Placement.Options, placement.Value.ToString(), placement.Callback);
                        break;
                    }
                    // TODO other config
                }
            });
            groups.Values.Do(group => {
                ModSettingAPI.AddGroup(group.Key, group.Description.ToLocalization(), group.Keys, group.Scale, false, group.Open);
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
                try {
                    item.SetValue(valueObject);
                }
                catch (Exception ex) {
                    Logger.Error($"Failed to Load {item.Key} : {valueObject.GetType()} {valueObject}", ex);
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
