using Duckov.PerkTrees;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(PerkTree), nameof(PerkTree.SetupSaveData))]
internal static class Patch_PerkTree_SetupSaveData {
    internal static readonly Dictionary<int, long> itemPerkCount = [];
    static readonly HashSet<string> CheckedPerkTrees = [];
    static readonly HashSet<Perk> LockedPerks = [];

    static void Postfix(PerkTree __instance) {
        if (CheckedPerkTrees.Contains(__instance.ID)) {
            Logger.Info("Clear old perk data");
            // 新存档，清除旧数据
            itemPerkCount.Clear();
            CheckedPerkTrees.Clear();
            foreach (var perk in LockedPerks) {
                if (perk == null) {
                    continue;
                }
                perk.onUnlockStateChanged -= OnPerkUnlocked;
            }
            LockedPerks.Clear();
        }
        Logger.Info($"Init perk tree {__instance.DisplayName}");
        CheckedPerkTrees.Add(__instance.ID);
        foreach (Perk? perk in __instance.Perks) {
            if (perk == null) {
                continue;
            }
            if (perk.Unlocked || perk.Unlocking) {
                continue;
            }
            LockedPerks.Add(perk);
            perk.onUnlockStateChanged += OnPerkUnlocked;
            foreach (var item in perk.Requirement.cost.items) {
                if (itemPerkCount.TryGetValue(item.id, out long preValue)) {
                    itemPerkCount[item.id] = preValue + item.amount;
                }
                else {
                    itemPerkCount[item.id] = item.amount;
                }
            }
        }
    }

    static void OnPerkUnlocked(Perk perk, bool _) {
        if (!LockedPerks.Contains(perk)) {
            return;
        }
        if (!perk.Unlocked & !perk.Unlocking) {
            return;
        }
        LockedPerks.Remove(perk);
        foreach (var item in perk.Requirement.cost.items) {
            if (itemPerkCount.TryGetValue(item.id, out long preValue)) {
                itemPerkCount[item.id] = preValue - item.amount;
            }
            else {
                Logger.Warn($"Perk {perk.DisplayName} requirement not recorded");
#if DEBUG
                var itemMetaData = ItemAssetsCollection.GetMetaData(item.id);
                Logger.Debug($"\t{itemMetaData.DisplayName} {item.amount}");
#endif
            }
        }
    }
}
