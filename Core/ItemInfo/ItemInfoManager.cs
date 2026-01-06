using Duckov.ItemUsage;
using Duckov.UI;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using ItemStatsSystem;
using System.Text;
using TMPro;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core.ItemInfo;

[NeedSetup]
internal static class ItemInfoManager {
    [ToggleConfig("ColoredInfo", "物品信息颜色")]
    public static bool EnableColoredInfo = true;
    [ToggleConfig("ItemCount", "物品已有数量")]
    public static bool EnableItemCount = true;
    [ToggleConfig("ItemDurability", "物品耐久")]
    public static bool EnableItemDurability = true;
    [ToggleConfig("ItemAvailableCount", "物品可用次数")]
    public static bool EnableItemAvailableCount = true;
    [ToggleConfig("ItemRequirement", "物品需求")]
    public static bool EnableItemRequirement = true;
    [ToggleConfig("ItemWeight", "物品重量")]
    public static bool EnableItemWeight = true;
    [ToggleConfig("ItemValue", "物品价值")]
    public static bool EnableItemValue = true;
    [ToggleConfig("ItemDecompose", "物品分解信息")]
    public static bool EnableItemDecompose = true;

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
        DestroyAllText();
    }

    static readonly ItemInfoUI ItemCountText = new();
    static readonly ItemInfoUI ItemDurabilityText = new();
    static readonly ItemInfoUI ItemRequirementText = new();
    static readonly ItemInfoUI ItemWeightText = new();
    static readonly ItemInfoUI ItemValueText = new();
    static readonly ItemInfoUI ItemDecomposeText = new();

    public static void HideAllText() {
        ItemDecomposeText.Hide();
        ItemValueText.Hide();
        ItemWeightText.Hide();
        ItemRequirementText.Hide();
        ItemDurabilityText.Hide();
        ItemCountText.Hide();
    }
    public static void DestroyAllText() {
        ItemDecomposeText.Destroy();
        ItemValueText.Destroy();
        ItemWeightText.Destroy();
        ItemRequirementText.Destroy();
        ItemDurabilityText.Destroy();
        ItemCountText.Destroy();
    }

    public static ItemInfoUI SetupItemCount(int typeID) {
        (int inStorage, int onPlayer, int onPet) = ItemUtils.GetItemCount(typeID);
        int total = inStorage + onPlayer + onPet;
        if (total == 0) {
            return ItemCountText.HideOnce();
        }
        return ItemCountText.UseCounter($"已有 {total}").AddPart("仓库", inStorage).AddPart("背包", onPlayer).AddPart("宠物", onPet);
    }
    public static ItemInfoUI SetupItemDurability(Item item) {
        if (!EnableItemDurability && !EnableItemAvailableCount) {
            return ItemCountText.HideOnce(); 
        }
        if (!item.UseDurability) {
            return ItemDurabilityText.HideOnce();
        }
        if (EnableItemDurability) {
            ItemDurabilityText.SetText($"耐久 {item.Durability:0.##} / {item.MaxDurabilityWithLoss:0.##}");
        }
        else {
            ItemDurabilityText.SetText("");
        }
        // 以下处理部分道具可用次数
        if (!EnableItemAvailableCount) {
            return ItemDurabilityText;
        }
        var usage = item.UsageUtilities;
        if (usage == null) {
            return ItemDurabilityText;
        }
        bool dynamicUsage = false;
        float useDurability = 0f;
        foreach (var behavior in usage.behaviors) {
            if (behavior == null) {
                continue;
            }
            switch (behavior) {
                case FoodDrink foodDrink: {
                    useDurability += foodDrink.UseDurability;
                    break;
                }
                case Drug drug: {
                    if (drug.useDurability) {
                        useDurability += drug.durabilityUsage; // 满治疗量消耗耐久
                        dynamicUsage = true;
                    }
                    break;
                }
                case RemoveBuff removeBuff: {
                    if (removeBuff.useDurability) {
                        useDurability += removeBuff.durabilityUsage;
                    }
                    break;
                }
            }
        }
        if (useDurability <= 1e-8) {
            return ItemDurabilityText;
        }
        int maxUseCount = Mathf.CeilToInt(item.MaxDurability / useDurability);
        int useCount = Mathf.CeilToInt(item.Durability / useDurability);
        return ItemDurabilityText.AppendTextIf(EnableItemDurability,"\n").AppendTextIf(dynamicUsage, "预估").AppendText($"可用次数 {useCount} / {maxUseCount}");
    }
    public static ItemInfoUI SetupItemRequirement(int typeID) {
        if (!EnableItemRequirement) {
            return ItemRequirementText.HideOnce();
        }
        int quest = ItemUtils.GetQuestRequirement(typeID);
        long building = ItemUtils.GetBuildingRequirement(typeID);
        long perk = ItemUtils.GetPerkRequirement(typeID);
        long total = quest + building + perk;
        if (total == 0) {
            return ItemRequirementText.HideOnce();
        }
        return ItemRequirementText.UseCounter($"需求 {total}").AddPart("任务", quest).AddPart("强化", perk).AddPart("建筑", building);
    }
    public static ItemInfoUI SetupItemDecompose(int typeID) {
        if (!EnableItemDecompose) {
            return ItemDecomposeText.HideOnce();
        }
        var decomposeItems = ItemUtils.GetDecomposeItems(typeID);
        if (decomposeItems.Length == 0) {
            return ItemDecomposeText.HideOnce();
        }
        StringBuilder stringBuilder = new("分解:\n", decomposeItems.Length + 1);
        foreach (var entry in decomposeItems) {
            var itemMetaData = ItemAssetsCollection.GetMetaData(entry.id);
            stringBuilder.AppendLine($"<indent=1em>{entry.amount}x {itemMetaData.DisplayName}</indent>");
        }
        return ItemDecomposeText.SetText(stringBuilder.ToString());
    }

    public static void OnSetupMetaHoveringUI(ItemHoveringUI uiInstance, ItemMetaData data) {
        HideAllText();

        var parent = uiInstance.LayoutParent;
        Color color = EnableColoredInfo ? data.GetLevelColor().WithAlpha(1f) : Color.white;

        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        if (EnableItemCount) {
            SetupItemCount(data.id).SetParent(parent).SetColor(color).Show();
        }
        SetupItemRequirement(data.id).SetParent(parent).SetColor(color).Show();

        if(EnableItemWeight){
            Item template = ItemAssetsCollection.GetPrefab(data.id);
            ItemWeightText.SetParent(parent).SetText($"单位重量 {template.UnitSelfWeight:0.##}kg").SetColor(color).Show();
        }
        if(EnableItemValue){
            ItemValueText.SetParent(parent).SetText($"${data.priceEach / 2f:0.##}").SetColor(color).Show();
        }
        SetupItemDecompose(data.id).SetParent(parent).SetColor(color).Show();
    }
    public static void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item? item) {
        HideAllText();
        if (item == null) {
            return;
        }
        var parent = uiInstance.LayoutParent;

        Color color = EnableColoredInfo ? item.GetLevelColor().WithAlpha(1f) : Color.white;

        Traverse.Create(uiInstance).Field("itemName").GetValue<TextMeshProUGUI>()?.color = color;

        if (EnableItemCount) {
            SetupItemCount(item.TypeID).SetParent(parent).SetColor(color).Show();
        }
        SetupItemDurability(item).SetParent(parent).SetColor(color).Show();
        SetupItemRequirement(item.TypeID).SetParent(parent).SetColor(color).Show();
        if(EnableItemWeight){
            ItemWeightText.SetParent(parent).SetText($"总重 {item.TotalWeight:0.##}kg").AppendTextIf(item.Slots != null && item.Slots.Count > 0, $"\t自重 {item.SelfWeight:0.##}kg").SetColor(color).Show();
        }
        if(EnableItemValue){
            ItemValueText.SetParent(parent).SetText($"${item.GetTotalRawValue() / 2f:0.##}").AppendTextIf(item.Stackable, $" ({item.Value / 2f:0.##})").SetColor(color).Show();
        }
        SetupItemDecompose(item.TypeID).SetParent(parent).SetColor(color).Show();
    }
}
