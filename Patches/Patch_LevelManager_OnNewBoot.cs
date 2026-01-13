using EnhancedItemInfo.Attributes;
using HarmonyLib;
using System;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(LevelManager), "OnNewBoot")]
public static class Patch_LevelManager_OnNewBoot {
    public static event Action? OnNewBoot = null;
    static void Postfix() {
        OnNewBoot?.Invoke();
    }
}
