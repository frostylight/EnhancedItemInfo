using Duckov.UI;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(MoneyDisplay))]
internal static class Patches_MoneyDisplay {
    [ToggleConfig("CashDisplay", "EnhancedItemInfo_Config_CashDisplay")]
    public static bool Enable = true;

    public static Dictionary<Item, int> CashCache = [];
    public static long CurrentCash = 0;

    public static TextMeshProUGUI? text = null;
    public static Image? icon = null;
    public static ComponentProxy? proxy = null;

    [HarmonyPostfix]
    [HarmonyPatch("OnEnable")]
    public static void Postafix_OnEnable(MoneyDisplay __instance, TextMeshProUGUI ___text) {
        OnDisable();
        if (!Enable) {
            if (text != null) {
                UnityEngine.Object.Destroy(text);
                text = null;
            }
            if (icon != null) {
                UnityEngine.Object.Destroy(icon);
                icon = null;
            }
            if (proxy != null) {
                UnityEngine.Object.Destroy(proxy);
                proxy = null;
            }
            return;
        }
        var parent = ___text.transform.parent;
        if (parent == null) {
            Logger.Error($"MoneyDisplay text not parent!");
            return;
        }
        Logger.Debug($"Enable MoneyDisplay {__instance.GetInstanceID()}");
        ItemUtilities.FindAllBelongsToPlayer(item => item != null && item.TypeID == Constant.CashTypeID)
            .ForEach(item => {
                RegisterCash(item);
                int count = item.StackCount;
                CashCache.Add(item, count);
                CurrentCash += count;
            });

        if (text == null) {
            text = UnityEngine.Object.Instantiate(___text, parent);
            text.gameObject.SetActive(false);
            text.gameObject.name = "CashText";
        }
        if (icon == null) {
            var image = parent.Find("Image")?.GetComponent<Image>();
            if (image != null) {
                icon = UnityEngine.Object.Instantiate(image, parent);
            }
            else {
                Logger.Warn($"MoneyDisplay Image not found");
                icon = new GameObject("CashIcon").AddComponent<Image>();
                icon.transform.localScale = text.transform.localScale;
            }
            icon.gameObject.SetActive(false);
            icon.gameObject.name = "CashIcon";
            icon.sprite = ItemAssetsCollection.GetPrefab(Constant.CashTypeID).Icon;
        }
        if (proxy == null) {
            proxy = new GameObject("MoneyDisplayProxy").AddComponent<ComponentProxy>();
            proxy.OnDisabled = OnDisable;
        }
        Refresh();
        proxy.transform.SetParent(parent, false);
        icon.transform.SetParent(parent, false);
        icon.transform.SetAsLastSibling();
        icon.gameObject.SetActive(true);
        text.transform.SetParent(parent, false);
        text.transform.SetAsLastSibling();
        text.gameObject.SetActive(true);
        PlayerStorage.Inventory?.onContentChanged += OnInventoryContentChanged;
        LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory?.onContentChanged += OnInventoryContentChanged;
        LevelManager.Instance?.PetProxy?.Inventory?.onContentChanged += OnInventoryContentChanged;
    }

    public static void OnDisable() {
        Logger.Debug($"MoneyDisplay.OnDisable");
        PlayerStorage.Inventory?.onContentChanged -= OnInventoryContentChanged;
        LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory?.onContentChanged -= OnInventoryContentChanged;
        LevelManager.Instance?.PetProxy?.Inventory?.onContentChanged -= OnInventoryContentChanged;

        CashCache.Keys?.Do(UnRegisterCash);
        CashCache.Clear();
        CurrentCash = 0;
    }

    public static void Refresh() {
        text?.text = $" {CurrentCash:n0}";
    }

    public static void RegisterCash(Item cash) {
        UnRegisterCash(cash);
        cash.onParentChanged += OnCashStatusChanged;
        cash.onSetStackCount += OnCashStatusChanged;
        cash.onDestroy += OnCashStatusChanged;
    }
    public static void UnRegisterCash(Item cash) {
        cash.onParentChanged -= OnCashStatusChanged;
        cash.onSetStackCount -= OnCashStatusChanged;
        cash.onDestroy -= OnCashStatusChanged;
    }
    public static void OnCashStatusChanged(Item cash) {
        if (!CashCache.TryGetValue(cash, out int count)) {
            Logger.Warn($"Not Cached Registered!");
            UnRegisterCash(cash);
            return;
        }
        if (cash.IsBeingDestroyed) {
            Logger.Debug($"Destroyed Cash");
            UnRegisterCash(cash);
            CashCache.Remove(cash);
            CurrentCash -= count;
            Refresh();
            return;
        }
        if (cash.TypeID != Constant.CashTypeID) {
            Logger.Warn($"Not Cash Item {cash.DisplayName}");
            UnRegisterCash(cash);
            CashCache.Remove(cash);
            CurrentCash -= count;
            Refresh();
            return;
        }
        if (cash.InInventory == null) {
            Logger.Debug($"Drop {count}");
            UnRegisterCash(cash);
            CashCache.Remove(cash);
            CurrentCash -= count;
            Refresh();
            return;
        }
        int current = cash.StackCount;
        Logger.Debug($"Cache {count} -> {current}");
        if (count == current) {
            return;
        }
        if (current > 0) {
            CashCache[cash] = current;
        }
        else {
            Logger.Debug($"\tRemove Cash");
            CashCache.Remove(cash);
        }
        CurrentCash += current - count;
        Refresh();
    }
    public static void OnInventoryContentChanged(Inventory inventory, int index) {
        if (inventory == null) {
            return;
        }
        if (index >= inventory.Content.Count) {
            return;
        }
        var item = inventory.Content[index];
        if (item == null) {
            return;
        }
        if (item.TypeID != Constant.CashTypeID) {
            return;
        }
        Logger.Debug($"{inventory.DisplayName} Add Cash {item.StackCount}");
        if (CashCache.ContainsKey(item)) {
            Logger.Debug($"\tAlready cache");
            return;
        }
        int count = item.StackCount;
        CurrentCash += count;
        CashCache.Add(item, count);
        RegisterCash(item);
        Refresh();
    }
}
