using Duckov.UI;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Text;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core.ItemInfo;

[SubModule]
internal static class ItemInfoManager {
    public static void Init() {
        Logger.Info($"{nameof(ItemInfoManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    public static void OnSetup() {
        Logger.Info($"{nameof(ItemInfoManager)} is enabled");

        ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
        ItemHoveringUI.onSetupMeta += OnSetupMetaHoveringUI;
    }
    public static void OnDeactivate() {
        Logger.Info($"{nameof(ItemInfoManager)} is disabled");

        ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
        ItemHoveringUI.onSetupMeta -= OnSetupMetaHoveringUI;
    }

    static readonly ItemInfoUI ItemCountText = new();
    static readonly ItemInfoUI ItemRequirementText = new();
    static readonly ItemInfoUI ItemWeightText = new();
    static readonly ItemInfoUI ItemValueText = new();
    static readonly ItemInfoUI ItemDecomposeText = new();

    public static void HideAllText() {
        ItemDecomposeText.Hide();
        ItemValueText.Hide();
        ItemWeightText.Hide();
        ItemRequirementText.Hide();
        ItemCountText.Hide();
    }

    public static ItemInfoUI SetupItemCount(int typeID) {
        (int inStorage, int onPlayer, int onPet) = ItemUtils.GetItemCount(typeID);
        int total = inStorage + onPlayer + onPet;
        if (total == 0) {
            return ItemCountText.HideOnce();
        }
        return ItemCountText.SetText($"已有{total} = 背包{onPlayer} + 宠物{onPet} + 仓库{inStorage}");
    }
    public static ItemInfoUI SetupItemRequirement(int typeID) {
        int quest = ItemUtils.GetQuestRequirement(typeID);
        long building = ItemUtils.GetBuildingRequirement(typeID);
        long perk = ItemUtils.GetPerkRequirement(typeID);
        long total = quest + building + perk;
        if (total == 0) {
            return ItemRequirementText.HideOnce();
        }
        return ItemRequirementText.SetText($"需求{total} = 任务{quest} + 强化{perk} + 建筑{building}");
    }
    public static ItemInfoUI SetupItemDecompose(int typeID) {
        var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
        if (decomposeItems.Length == 0) {
            return ItemDecomposeText.HideOnce();
        }
        StringBuilder stringBuilder = new("分解：", decomposeItems.Length + 1);
        foreach (var entry in decomposeItems) {
            var itemMetaData = ItemAssetsCollection.GetMetaData(entry.id);
            stringBuilder.Append($"\n\t{entry.amount}x {itemMetaData.DisplayName}");
        }
        return ItemDecomposeText.SetText(stringBuilder.ToString());
    }
    public static void OnSetupMetaHoveringUI(ItemHoveringUI uiInstance, ItemMetaData data) {
        HideAllText();

        var parent = uiInstance.LayoutParent;
        Color color = data.GetLevelColor().WithAlpha(1f);

        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        SetupItemCount(data.id).SetParent(parent).SetColor(color).Show();
        SetupItemRequirement(data.id).SetParent(parent).SetColor(color).Show();

        Item template = ItemAssetsCollection.GetPrefab(data.id);
        ItemWeightText.SetParent(parent).SetText($"单位重量{template.UnitSelfWeight:0.##}kg").SetColor(color).Show();

        ItemValueText.SetParent(parent).SetText($"${data.priceEach / 2}").SetColor(color).Show();
        SetupItemDecompose(data.id).SetParent(parent).SetColor(color).Show();
    }
    public static void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item? item) {
        HideAllText();
        if (item == null) {
            return;
        }
        var parent = uiInstance.LayoutParent;
        Color color = item.GetLevelColor().WithAlpha(1f);

        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        SetupItemCount(item.TypeID).SetParent(parent).SetColor(color).Show();
        SetupItemRequirement(item.TypeID).SetParent(parent).SetColor(color).Show();
        ItemWeightText.SetParent(parent).SetText($"总重{item.TotalWeight:0.##}kg").AppendTextIf(item.Slots != null && item.Slots.Count > 0, $"\t自重{item.SelfWeight:0.##}kg").SetColor(color).Show();
        ItemValueText.SetParent(parent).SetText($"${item.GetTotalRawValue() / 2}").AppendTextIf(item.Stackable, $" ({item.Value / 2})").SetColor(color).Show();
        SetupItemDecompose(item.TypeID).SetParent(parent).SetColor(color).Show();
    }
}
