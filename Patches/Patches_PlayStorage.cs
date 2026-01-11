using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(PlayerStorage))]
internal static class Patches_PlayStorage {
    public static readonly Dictionary<int, int> ItemCountCache = [];
    public static bool Cache { get; private set; }

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
