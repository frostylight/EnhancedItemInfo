using Duckov.Modding;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Localization;

public static class LocalizationManager {
    public static bool Inited { get; private set; } = false;

    public static SystemLanguage CurrentLanguage => SodaCraft.Localizations.LocalizationManager.CurrentLanguage;
    public static SystemLanguage FallbackLanguage => CurrentLanguage switch {
        SystemLanguage.Chinese => SystemLanguage.ChineseSimplified,
        SystemLanguage.ChineseTraditional => SystemLanguage.ChineseSimplified,
        _ => SystemLanguage.English,
    };

    static string path = "Localization";

    static readonly Dictionary<string, string> Localizations = [];

    public static void Init(ModInfo info) {
        if (Inited) {
            return;
        }
        path = Path.Combine(info.path, "Localization");
        LoadFromFile();
        SodaCraft.Localizations.LocalizationManager.OnSetLanguage += OnSetLanguage;
        Inited = true;
    }

    public static string GetLocalization(string key) {
        if (Localizations.TryGetValue(key, out var result)) {
            if (!string.IsNullOrEmpty(result)) {
                return result;
            }
        }
        return key.ToPlainText();
    }

    static void LoadFromFile() {
        Localizations.Clear();
        var csv = Path.Combine(path, $"{CurrentLanguage}.csv");
        if (!File.Exists(csv)) {
            Logger.Warn($"Localization File not found : {csv}");
            csv = Path.Combine(path, $"{FallbackLanguage}.csv");
            Logger.Info($"Fallback to {FallbackLanguage}");
            if (!File.Exists(csv)) {
                Logger.Error($"Fallback Localization File not found : {csv}");
                return;
            }
        }
        try {
            using StreamReader stream = File.OpenText(csv);
            stream.ReadLine(); // 跳过表头
            while (!stream.EndOfStream) {
                var line = stream.ReadLine().Trim();
                if (string.IsNullOrEmpty(line)) {
                    continue;
                }
                if (line.StartsWith(';')) {
                    continue;
                }
                var keyValue = line.Split(",");
                if (keyValue.Length < 2) {
                    Logger.Warn($"Incomplete Localization Found: {line}");
                    continue;
                }
                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();
                if (string.IsNullOrEmpty(key)) {
                    Logger.Warn($"Empty Key Found : {line}");
                    continue;
                }
                if (keyValue.Length > 2) {
                    Logger.Warn($"Extra Data Found : {keyValue.Skip(2)}");
                }
                if (Localizations.ContainsKey(key)) {
                    Logger.Warn($"Dulicated Localization Key overwrite {key} : {Localizations[key]} -> {value}");
                }
                Localizations[key] = value;
            }
        }
        catch (Exception ex) {
            Logger.Error($"Error on Reading Localization File : {csv}", ex);
        }
    }

    static void OnSetLanguage(SystemLanguage language) {
        LoadFromFile();
    }
}
