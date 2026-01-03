using Duckov.Buildings;
using Duckov.Economy;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patchs;

[PatchNeedSetup]
[HarmonyPatch(typeof(BuildingManager))]
internal class Patch_BuildingManager {
    // 每种物品的建筑需求
    public static readonly Dictionary<int, long> itemBuildingCount = [];

    public static void Init() {
        Logger.Info($"{nameof(Patch_BuildingManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    public static void OnSetup() {
        Logger.Info($"{nameof(Patch_BuildingManager)} is enabled");

        BuildingManager.OnBuildingBuiltComplex += OnBuildingBuilt;
    }
    public static void OnDeactivate() {
        Logger.Info($"{nameof(Patch_BuildingManager)} is disabled");

        BuildingManager.OnBuildingBuiltComplex -= OnBuildingBuilt;
    }

    static void AddCost(Cost cost, int amount) {
        foreach (var item in cost.items) {
            if (itemBuildingCount.TryGetValue(item.id, out long preValue)) {
                itemBuildingCount[item.id] = preValue + item.amount * amount;
            }
            else {
                if (amount < 0) {
                    Logger.Warn($"Building requirement not recorded!");
#if DEBUG
                    var itemMetaData = ItemAssetsCollection.GetMetaData(item.id);
                    Logger.Debug($"\t{itemMetaData.DisplayName} {item.amount}");
#endif
                }
                itemBuildingCount[item.id] = item.amount * amount;
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
        AddCost(info.cost, -1);
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
            AddCost(info.cost, amount);
        }
    }
}
