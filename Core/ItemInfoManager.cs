using Duckov.UI;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Core.ItemInfo;
using EnhancedItemInfo.Core.ItemLevel;
using EnhancedItemInfo.Extensions;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core;

[NeedSetup]
internal static class ItemInfoManager {
    [ToggleConfig("ColoredInfo", "物品信息颜色")]
    public static bool EnableColoredInfo = true;

    [ToggleConfig("ItemProperties", "显示物品参数（黑市）")]
    public static bool EnableItemProperties = true;
    static FieldInfo? FieldItemProperties = null;
    static Action<ItemPropertiesDisplay, Item>? SetupItemProperties = null;

    public static void Init() {
        Logger.Info($"{nameof(ItemInfoManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;

        FieldItemProperties = AccessTools.DeclaredField(typeof(ItemHoveringUI), "itemProperties");
        if (FieldItemProperties == null) {
            Logger.Error($"Failed to get ItemHoveringUI.itemProperties");
        }
        var method = AccessTools.DeclaredMethod(typeof(ItemPropertiesDisplay), "Setup", [typeof(Item)]);
        if (method == null) {
            Logger.Error($"Failed to get ItemPropertiesDisplay");
        }
        else {
            SetupItemProperties = (Action<ItemPropertiesDisplay, Item>)method.CreateDelegate(typeof(Action<ItemPropertiesDisplay, Item>));
        }
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

        Color color = EnableColoredInfo ? data.GetMeta().GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;


        if (EnableItemProperties) {
            do {
                if (FieldItemProperties == null || SetupItemProperties == null) {
                    EnableItemProperties = false;
                    break;
                }
                try {
                    var itemProperties = (ItemPropertiesDisplay)FieldItemProperties.GetValue(uiInstance);
                    var prefab = ItemAssetsCollection.GetPrefab(data.id);
                    SetupItemProperties(itemProperties, prefab);
                    itemProperties.gameObject.SetActive(true);
                }
                catch (Exception ex) {
                    Logger.Error($"Failed to setup ItemPropertiesDisplay", ex);
                    EnableItemProperties = false;
                }
            } while (false);
        }

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

        Color color = EnableColoredInfo ? item.GetMeta().GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        ItemCount.Instance.SetupAndShow(uiInstance, item);
        ItemDurability.Instance.SetupAndShow(uiInstance, item);
        ItemRequirement.Instance.SetupAndShow(uiInstance, item);
        ItemWeight.Instance.SetupAndShow(uiInstance, item);
        ItemValue.Instance.SetupAndShow(uiInstance, item);
        ItemDecompose.Instance.SetupAndShow(uiInstance, item);
    }
}
