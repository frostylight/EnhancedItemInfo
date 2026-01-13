using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patches;

[Patch]
[NeedSetup]
[HarmonyPatch(typeof(PlayerStorage))]
internal static class Patches_PlayStorage {
    public static readonly Dictionary<int, int> ItemCountCache = [];
    public static bool Cache { get; private set; }

    public static void Init() {
        Logger.Info($"{nameof(Patches_PlayStorage)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }

    public static void OnSetup() {
        Logger.Info($"{nameof(Patches_PlayStorage)} is enabled");

        Patch_LevelManager_OnNewBoot.OnNewBoot += OnNewBoot;
    }

    public static void OnDeactivate() {
        Logger.Info($"{nameof(Patches_PlayStorage)} is disabled");

        Patch_LevelManager_OnNewBoot.OnNewBoot -= OnNewBoot;
    }

    public static void OnNewBoot() {
        Logger.Debug($"Clear cache before new boot");
        Cache = false;
        ItemCountCache.Clear();
    }

    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    public static void Postfix_Awake() {
        Logger.Debug($"Clear cache after PlayerStorage.Awake");
        ItemCountCache.Clear();
        Cache = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("OnDestroy")]
    public static void Prefix_OnDestroy() {
        Logger.Debug($"Caching before PlayerStorage.OnDestroy");
        var storage = PlayerStorage.Inventory;
        if (storage == null) {
            Logger.Error($"Null PlayerStorage Inventory before destroyed!");
            return;
        }
        Cache = true;
        ItemCountCache.Clear();
        storage.Content.DoIf(item => item != null && item,
            item => {
                if (ItemCountCache.TryGetValue(item.TypeID, out int count)) {
                    ItemCountCache[item.TypeID] = count + item.StackCount;
                }
                else {
                    ItemCountCache[item.TypeID] = item.StackCount;
                }
            });
    }
}
