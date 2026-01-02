using Duckov.UI;
using EnhancedItemInfo.Core.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Text;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Core.Utils.Logger;

namespace EnhancedItemInfo.Core.ItemInfo;

[SubModule]
internal static class ItemInfoManager {
    public static void Init() {
        Logger.Info("ItemInfo is registered");
        ModBehaviour.OnSetupSubmodule += OnSetup;
        ModBehaviour.OnDeactivateSubModule += OnDeactivate;
    }

    public static void OnSetup() {
        Logger.Info("ItemInfo is enabled");

        ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
        ItemHoveringUI.onSetupMeta += OnSetupMetaHoveringUI;
    }
    public static void OnDeactivate() {
        Logger.Info("ItemInfo is disabled");

        ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
        ItemHoveringUI.onSetupMeta -= OnSetupMetaHoveringUI;
    }

    static readonly ItemInfoUI ItemCountText = new();
    static readonly ItemInfoUI ItemWeightText = new();
    static readonly ItemInfoUI ItemValueText = new();
    static readonly ItemInfoUI ItemDecomposeText = new();

    public static void HideAllText() {
        ItemValueText.Hide();
        ItemWeightText.Hide();
        ItemCountText.Hide();
        ItemDecomposeText.Hide();
    }

    public static ItemInfoUI SetupItemCount(int typeID) {
        (int inStorage, int onPlayer) = ItemUtils.GetItemCount(typeID);
        int total = inStorage + onPlayer;
        if (total == 0) {
            return ItemCountText.HideOnce();
        }
        return ItemCountText.SetText($"已有{total} = 背包{onPlayer} + 仓库{inStorage}");
    }
    public static ItemInfoUI SetupItemDecompose(int typeID) {
        var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
        if (decomposeItems.Length == 0) {
            return ItemDecomposeText.HideOnce();
        }
        StringBuilder stringBuilder = new("分解：", decomposeItems.Length  + 1);
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

        Item template = ItemAssetsCollection.GetPrefab(data.id);
        ItemWeightText.SetParent(parent).SetText($"单位重量：{template.UnitSelfWeight:0.##}kg").SetColor(color).Show();

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
        ItemWeightText.SetParent(parent).SetText($"总重：{item.TotalWeight:0.##}kg").AppendTextIf(item.Slots != null && item.Slots.Count > 0, $"\t\t自重：{item.SelfWeight:0.##}kg").SetColor(color).Show();
        ItemValueText.SetParent(parent).SetText($"${item.GetTotalRawValue() / 2}").AppendTextIf(item.Stackable, $" ({item.Value / 2})").SetColor(color).Show();
        SetupItemDecompose(item.TypeID).SetParent(parent).SetColor(color).Show();
    }
}
