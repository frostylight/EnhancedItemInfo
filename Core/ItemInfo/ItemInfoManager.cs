using Duckov.UI;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core.ItemInfo;

[NeedSetup]
internal static class ItemInfoManager {
    [ToggleConfig("ColoredInfo", "物品信息颜色")]
    public static bool EnableColoredInfo = true;

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

    public static void HideAllText() {
        ItemDecompose.Instance.Hide();
        ItemValue.Instance.Hide();
        ItemWeight.Instance.Hide();
        ItemRequirement.Instance.Hide();
        ItemDurability.Instance.Hide();
        ItemCount.Instance.Hide();
    }

    public static void OnSetupMetaHoveringUI(ItemHoveringUI uiInstance, ItemMetaData data) {
        HideAllText();

        Color color = EnableColoredInfo ? data.GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        ItemCount.Instance.SetupAndShow(uiInstance, data);
        ItemRequirement.Instance.SetupAndShow(uiInstance, data);
        ItemWeight.Instance.SetupAndShow(uiInstance, data);
        ItemValue.Instance.SetupAndShow(uiInstance, data);
        ItemDecompose.Instance.SetupAndShow(uiInstance, data);
    }
    public static void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item? item) {
        HideAllText();
        if (item == null) {
            return;
        }

        Color color = EnableColoredInfo ? item.GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        ItemCount.Instance.SetupAndShow(uiInstance, item);
        ItemDurability.Instance.SetupAndShow(uiInstance, item);
        ItemRequirement.Instance.SetupAndShow(uiInstance, item);
        ItemWeight.Instance.SetupAndShow(uiInstance, item);
        ItemValue.Instance.SetupAndShow(uiInstance, item);
        ItemDecompose.Instance.SetupAndShow(uiInstance, item);
    }
}
