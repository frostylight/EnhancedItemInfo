using EnhancedItemInfo.Core;
using EnhancedItemInfo.Core.Utils;
using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedItemInfo;

public class ModBehaviour: Duckov.Modding.ModBehaviour {
    public const string ModId = $"{VersionInfo.Author}.{VersionInfo.Name}";

    internal static Harmony? harmony = null;
    public static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    static bool Inited = false;
    internal static event Action? OnSetupSubmodule = null;
    internal static event Action? OnDeactivateSubModule = null;

    static void Init() {
        if (Inited) {
            return;
        }
        Logger.Info("Init submodule");
        foreach (var type in AccessTools.GetTypesFromAssembly(assembly)) {
            if (type == null) {
                continue;
            }
            if (!type.IsDefined(typeof(SubModuleAttribute), false)) {
                continue;
            }
            try {
                AccessTools.Method(type, "Init").Invoke(null, []);
            }
            catch (Exception ex) {
                Logger.Error($"Unable to Init {type.FullName}", ex);
            }
        }
        Inited = true;
    }

    protected override void OnAfterSetup() {
        base.OnAfterSetup();

        Init();

        Logger.Info("Loading submodule");

        OnSetupSubmodule?.Invoke();

        try {
            harmony = new Harmony(ModId);
            harmony?.PatchAll(assembly);
        }
        catch (Exception ex) {
            Logger.Error("Failed to patch harmony", ex);
        }
    }
    protected override void OnBeforeDeactivate() {
        base.OnBeforeDeactivate();

        Logger.Info("Disabling submodule");

        OnDeactivateSubModule?.Invoke();

        try {
            harmony?.UnpatchAll(ModId);
        }
        catch (Exception ex) {
            Logger.Error("Failed to unpatch harmony", ex);
        }
    }

    void OnDestroy() {
        OnBeforeDeactivate();
    }
}
