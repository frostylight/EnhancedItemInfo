using Duckov.Economy;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(DecomposeDatabase), nameof(DecomposeDatabase.RebuildDictionary))]
internal static class Patch_DecomposeDatabase_RebuildDictionary {
    public static bool Cache = false;
    public static Dictionary<int, DecomposeFormula> DecomposeFormulaCache = [];
    public static Dictionary<int, Dictionary<int, long>> DecomposeFromCache = [];

    public static Cost.ItemEntry[] GetFormulaByTypeID(int typeID) {
        if (!Cache) {
            DecomposeDatabase.Instance?.RebuildDictionary();
        }
        if (DecomposeFormulaCache.TryGetValue(typeID, out var formula)) {
            if (formula.valid) {
                return formula.result.items;
            }
        }
        return [];
    }
    public static Dictionary<int, long> GetDecomposeFromByTypeID(int typeID) {
        if (!Cache) {
            DecomposeDatabase.Instance?.RebuildDictionary();
        }
        if (DecomposeFromCache.TryGetValue(typeID, out var dict)) {
            return dict;
        }
        return [];
    }

    static void Postfix(Dictionary<int, DecomposeFormula> ____dic) {
        Logger.Info($"Refresh decompose data");
        Cache = false;
        DecomposeFromCache.Clear();
        DecomposeFormulaCache = ____dic; //直接拿引用节省空间
        foreach (var (fromItem, formula) in ____dic) {
            foreach (var toItem in formula.result.items) {
                DecomposeFromCache.GetOrCreate(toItem.id)
                    .Add(fromItem, toItem.amount);
            }
        }
        Cache = true;
    }
}
