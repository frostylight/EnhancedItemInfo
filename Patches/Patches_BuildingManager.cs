using Duckov.Buildings;
using Duckov.Economy;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Extensions;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedItemInfo.Patches;

[Patch]
[NeedSetup]
[HarmonyPatch(typeof(BuildingManager))]
internal class Patches_BuildingManager {
    // 每种物品的建筑需求
    static readonly Dictionary<int, Dictionary<string, long>> itemBuildingCount = [];
    static readonly Dictionary<string, string> BuildingName = [];

    public static IEnumerable<(string Name, long Amount)> GetBuildingRequirement(int typeID) {
        if (itemBuildingCount.TryGetValue(typeID, out var dict)) {
            return dict.AsEnumerable().Select(kv => (Name: BuildingName[kv.Key], Amount: kv.Value));
        }
        return [];
    }

    public static void Init() {
        Logger.Info($"{nameof(Patches_BuildingManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    public static void OnSetup() {
        Logger.Info($"{nameof(Patches_BuildingManager)} is enabled");

        BuildingManager.OnBuildingBuiltComplex += OnBuildingBuilt;
    }
    public static void OnDeactivate() {
        Logger.Info($"{nameof(Patches_BuildingManager)} is disabled");

        BuildingManager.OnBuildingBuiltComplex -= OnBuildingBuilt;
    }

    static void AddCost(string id, Cost cost, int scale) {
        foreach (var item in cost.items) {
            var dict = itemBuildingCount.GetOrCreate(item.id);
            if (dict.TryGetValue(id, out var amount)) {
                dict[id] = amount + item.amount * scale;
            }
            else {
                if (scale < 0) {
                    Logger.Warn($"Building Requirement not recorded");
                }
                dict.Add(id, item.amount * scale);
            }
        }
    }

    static bool useToken = false;
    [HarmonyPrefix]
    [HarmonyPatch("BuyAndPlace")]
    static void Prefix_BuyAndPlace(string id) {
        useToken = BuildingManager.Instance.GetTokenAmount(id) > 0;
    }
    static void OnBuildingBuilt(int guid, BuildingInfo info) {
        Logger.Debug($"Build {info.DisplayName}");
        if (info.maxAmount <= 0) {
            // 不计算可无限建造的建筑
            return;
        }
        if (useToken) {
            // 重建不消耗资源
            return;
        }
        AddCost(info.id, info.cost, -1);
    }

    [HarmonyPostfix]
    [HarmonyPatch("Load")]
    static void Postfix_Load() {
        Logger.Info("Init building data");
        itemBuildingCount.Clear();

        foreach (var info in BuildingDataCollection.Instance.Infos) {
            if (!info.Valid) {
                continue;
            }
            Logger.Debug($"Handle Building {info.id} {info.DisplayName}");
            if (info.maxAmount <= 0) {
                // 不计算可无限建造的建筑
                continue;
            }
            int tokenAmount = BuildingManager.Instance.GetTokenAmount(info.id); // 拆除未重建数量
            int amount = info.maxAmount - info.CurrentAmount - tokenAmount;
            if (amount <= 0) {
                // 到达数量上限
                continue;
            }
            BuildingName[info.id] = info.DisplayName;
            AddCost(info.id, info.cost, amount);
        }
    }
}
