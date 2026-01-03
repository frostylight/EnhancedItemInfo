using Duckov.UI;
using EnhancedItemInfo.Patchs;
using EnhancedItemInfo.Utils;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core.RegisteredMark;

[SubModule]
internal static class RegisteredMarkManager {
    public static void Init() {
        Logger.Info($"{nameof(RegisteredMarkManager)} is registered");

        ModBehaviour.OnSetup += OnSetup;
        ModBehaviour.OnDeactivate += OnDeactivate;
    }
    public static void OnSetup() {
        Logger.Info($"{nameof(RegisteredMarkManager)} is enabled");

        Patch_ItemDisplay_Setup.OnItemDisplayReset += OnItemDisplayReset;
        Patch_ItemDisplay_Setup.OnItemDisplayShow += OnItemDisplayShow;
        Patch_ItemMetaDisplay_Setup.OnItemMetaDisplayShow += OnItemMetaDisplayShow;
        Patch_ItemAmountDisplay_Setup.OnItemAmountDisplayShow += OnItemAmountDisplayShow;
    }
    public static void OnDeactivate() {
        Logger.Info($"{nameof(RegisteredMarkManager)} is disabled");

        Patch_ItemDisplay_Setup.OnItemDisplayReset -= OnItemDisplayReset;
        Patch_ItemDisplay_Setup.OnItemDisplayShow -= OnItemDisplayShow;
        Patch_ItemMetaDisplay_Setup.OnItemMetaDisplayShow -= OnItemMetaDisplayShow;
        Patch_ItemAmountDisplay_Setup.OnItemAmountDisplayShow -= OnItemAmountDisplayShow;
    }

    public static void SetupMarkBackground(TextMeshProUGUI textui) {
        textui.text = "●";
        textui.fontSize = 32f;
        textui.color = Constant.MarkBackgroundColor;
        textui.alignment = TextAlignmentOptions.Center;
        textui.raycastTarget = false;

        var textuiRT = textui.rectTransform;
        textuiRT.anchorMin = Vector2.one;
        textuiRT.anchorMax = Vector2.one;
        textuiRT.pivot = Vector2.one;
        textuiRT.anchoredPosition = Constant.MarkPostion;
        textuiRT.sizeDelta = Constant.MarkSize;
    }
    public static void SetupMark(TextMeshProUGUI textui) {
        textui.text = "★";
        textui.fontSize = 20f;
        textui.color = Constant.MarkColor;
        textui.alignment = TextAlignmentOptions.Center;
        textui.raycastTarget = false;

        var textuiRT = textui.rectTransform;
        textuiRT.anchorMin = Vector2.one;
        textuiRT.anchorMax = Vector2.one;
        textuiRT.pivot = Vector2.one;
        textuiRT.anchoredPosition = Constant.MarkPostion;
        textuiRT.sizeDelta = Constant.MarkSize;
    }
    public static void SetupAndShow(Transform background) {
        var markBgTf = background.Find(Constant.RegisteredMarkBackgroundKey);
        if (markBgTf != null) {
            markBgTf.gameObject.SetActive(true);
        }
        else {
            var markBgGO = new GameObject(Constant.RegisteredMarkBackgroundKey);
            markBgGO.transform.SetParent(background, false);
            markBgGO.transform.localScale = Vector3.one;
            SetupMarkBackground(markBgGO.AddComponent<TextMeshProUGUI>());
            markBgGO.SetActive(true);
        }

        var markTextTf = background.Find(Constant.RegisteredMarkTextKey);
        if (markTextTf != null) {
            markTextTf.gameObject.SetActive(true);
        }
        else {
            var markTextGO = new GameObject(Constant.RegisteredMarkTextKey);
            markTextGO.transform.SetParent(background, false);
            markTextGO.transform.localScale = Vector3.one;
            SetupMark(markTextGO.AddComponent<TextMeshProUGUI>());
            markTextGO.SetActive(true);
        }
    }
    public static void HideMark(Transform background) {
        var markBg = background.Find(Constant.RegisteredMarkBackgroundKey);
        markBg?.gameObject.SetActive(false);
        var markText = background.Find(Constant.RegisteredMarkTextKey);
        markText?.gameObject.SetActive(false);
    }

    public static void OnItemDisplayReset(ItemDisplay itemDisplay) {
        var background = itemDisplay.transform?.Find("BG");
        if (background == null) {
            Logger.Warn($"Null background of {itemDisplay.name}");
            return;
        }
        HideMark(background);
    }
    public static void OnItemDisplayShow(ItemDisplay itemDisplay) {
        if (!itemDisplay.Target.IsRegistered()) {
            OnItemDisplayReset(itemDisplay);
            return;
        }
        var background = itemDisplay.transform?.Find("BG");
        if (background == null) {
            Logger.Warn($"Null background of {itemDisplay.name}");
            return;
        }
        SetupAndShow(background);
    }
    public static void OnItemMetaDisplayShow(ItemMetaDisplay itemMetaDisplay) {
        var background = itemMetaDisplay.transform?.Find("BG");
        if (background == null) {
            Logger.Warn($"Null background of {itemMetaDisplay.name}");
            return;
        }
        if (!itemMetaDisplay.GetMetaData().IsRegistered()) {
            HideMark(background);
            return;
        }
        SetupAndShow(background);
    }
    public static void OnItemAmountDisplayShow(ItemAmountDisplay itemAmountDisplay) {
        var icon = itemAmountDisplay.transform?.Find("Icon");
        if (icon == null) {
            Logger.Warn($"Null Icon of {itemAmountDisplay.name}");
            return;
        }
        if (!itemAmountDisplay.GetMetaData().IsRegistered()) {
            HideMark(icon);
            return;
        }
        SetupAndShow(icon);
    }
}
