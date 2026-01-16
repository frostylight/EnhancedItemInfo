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
[ConfigGroup("ItemInfo", "EnhancedItemInfo_Config_ItemInfo", 0.5f, true)]
internal static class ItemInfoManager {
    [ToggleConfig("ColoredInfo", "EnhancedItemInfo_Config_ColoredInfo")]
    public static bool EnableColoredInfo = true;

    [SliderConfig("InfoFontSize", "EnhancedItemInfo_Config_FontSize", 15f, 30f, nameof(OnFontSizeValueChanged))]
    public static float FontSize = 20f;
    public static event Action<float>? OnFontSizeChanged = null;
    static void OnFontSizeValueChanged(float fontSize) {
        OnFontSizeChanged?.Invoke(fontSize);
    }

    [ToggleConfig("ItemProperties", "EnhancedItemInfo_Config_ItemProperties")]
    public static bool EnableItemProperties = true;
    static FieldInfo? FieldItemProperties = null;
    static Action<ItemPropertiesDisplay, Item>? SetupItemProperties = null;

    public static SidePanel Panel { get => field ??= new(); } = null;

    public static event Action<ItemHoveringUI, Item>? OnSetupItem = null;
    public static event Action<ItemHoveringUI, ItemMetaData>? OnSetupMeta = null;

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

        OnSetupItem += ItemCount.Instance.SetupAndShow;
        OnSetupItem += ItemDurability.Instance.SetupAndShow;
        OnSetupItem += ItemRequirement.Instance.SetupAndShow;
        OnSetupItem += ItemWeight.Instance.SetupAndShow;
        OnSetupItem += ItemValue.Instance.SetupAndShow;
        OnSetupItem += ItemDecompose.Instance.SetupAndShow;
        OnSetupItem += ItemDecomposeFrom.Instance.SetupAndShow;

        OnSetupMeta += ItemCount.Instance.SetupAndShow;
        OnSetupMeta += ItemDurability.Instance.SetupAndShow;
        OnSetupMeta += ItemRequirement.Instance.SetupAndShow;
        OnSetupMeta += ItemWeight.Instance.SetupAndShow;
        OnSetupMeta += ItemValue.Instance.SetupAndShow;
        OnSetupMeta += ItemDecompose.Instance.SetupAndShow;
        OnSetupMeta += ItemDecomposeFrom.Instance.SetupAndShow;
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

    public static void OnSetupMetaHoveringUI(ItemHoveringUI uiInstance, ItemMetaData data) {
        Color color = EnableColoredInfo ? data.GetMeta().GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        if (EnableItemProperties) {
            if (FieldItemProperties == null || SetupItemProperties == null) {
                EnableItemProperties = false;
            }
            else {
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
            }
        }

        Panel.Hide();
        OnSetupMeta?.Invoke(uiInstance, data);
    }
    public static void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item? item) {
        if (item == null) {
            return;
        }

        Color color = EnableColoredInfo ? item.GetMeta().GetLevelColor().WithAlpha(1f) : Color.white;
        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        Panel.Hide();
        OnSetupItem?.Invoke(uiInstance, item);
    }
}
