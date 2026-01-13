using Duckov.PerkTrees;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(PerkTree), nameof(PerkTree.SetupSaveData))]
internal static class Patch_PerkTree_SetupSaveData {
    static readonly Dictionary<int, Dictionary<Perk, long>> itemPerkCount = [];
    static readonly HashSet<string> CheckedPerkTrees = [];
    static readonly HashSet<Perk> LockedPerks = [];

    public static IEnumerable<(string Name, long Amount)> GetPerkRequirement(int typeID) {
        if (itemPerkCount.TryGetValue(typeID, out var dict)) {
            return dict.AsEnumerable().Select(kv => (Name: $"{kv.Key.Master.DisplayName}.{kv.Key.DisplayName}", Amount: kv.Value));
        }
        return [];
    }

    static void Postfix(PerkTree __instance) {
        if (CheckedPerkTrees.Contains(__instance.ID)) {
            Logger.Info("Clear old perk data");
            // 清除旧数据
            itemPerkCount.Clear();
            CheckedPerkTrees.Clear();
            foreach (var perk in LockedPerks) {
                perk?.onUnlockStateChanged -= OnPerkUnlocked;
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
            Logger.Debug($"\tPerk {perk.DisplayName}");
            foreach (var item in perk.Requirement.cost.items) {
#if DEBUG
                var itemMetaData = ItemAssetsCollection.GetMetaData(item.id);
                Logger.Debug($"\t\t{itemMetaData.DisplayName} {item.amount}");
#endif
                var dict = itemPerkCount.GetOrCreate(item.id);
                if (dict.TryGetValue(perk, out long amount)) {
                    dict[perk] = amount + item.amount;
                }
                else {
                    dict.Add(perk, item.amount);
                }
            }
        }
    }

    static void OnPerkUnlocked(Perk perk, bool _) {
        if (!LockedPerks.Contains(perk)) {
            return;
        }
        if (!perk.Unlocked && !perk.Unlocking) {
            return;
        }
        LockedPerks.Remove(perk);
        foreach (var item in perk.Requirement.cost.items) {
            itemPerkCount.GetOrCreate(item.id).Remove(perk);
        }
    }
}
