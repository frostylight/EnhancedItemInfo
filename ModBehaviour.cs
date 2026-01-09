using EnhancedItemInfo.Config;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnhancedItemInfo;

public class ModBehaviour: Duckov.Modding.ModBehaviour {
    public const string ModId = $"{VersionInfo.Author}.{VersionInfo.Name}";

    internal static Harmony? HarmonyInstance {
        get {
            if (field == null) {
                try {
                    field = new Harmony(ModId);
                }
                catch (Exception ex) {
                    Logger.Error("Failed to create harmony instance", ex);
                }
            }
            return field;
        }
    } = null;
    public static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    static bool Inited = false;
    internal static event Action? OnSetup = null;
    internal static event Action? OnDeactivate = null;

    internal static List<Type> Patchs = [];

    void Init() {
        if (Inited) {
            return;
        }
        Logger.Info("Init submodule & patch");
        foreach (var type in AccessTools.GetTypesFromAssembly(assembly)) {
            if (type == null) {
                continue;
            }
            if (type.NeedSetup()) {
                try {
                    AccessTools.DeclaredMethod(type, "Init").Invoke(null, []);
                }
                catch (Exception ex) {
                    Logger.Error($"Unable to Init {type.FullName}", ex);
                }
            }
            if (type.IsPatch()) {
                Patchs.Add(type);
            }
        }
        Logger.Info($"Init Setting");
        ConfigManager.Init(info);
        Inited = true;
    }

    protected override void OnAfterSetup() {
        base.OnAfterSetup();

        Init();

        Logger.Info("Loading submodule");

        OnSetup?.Invoke();

        if (HarmonyInstance == null) {
            return;
        }
        foreach (var patch in Patchs) {
            // 分离Patch避免整个Mod挂了
            try {
                HarmonyInstance.CreateClassProcessor(patch).Patch();
                Logger.Info($"Patch {patch.FullName} success");
            }
            catch (Exception ex) {
                Logger.Error($"Failed to patch {patch.FullName}", ex);
            }
        }
    }
    protected override void OnBeforeDeactivate() {
        base.OnBeforeDeactivate();

        Logger.Info("Disable All");

        HarmonyInstance?.UnpatchAll(ModId);

        OnDeactivate?.Invoke();
    }

    public void OnApplicationQuit() {
        Logger.Info($"Save setting before quit");

        ConfigManager.Save();
    }
}
