using Duckov.Economy;
using Duckov.Utilities;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Patchs;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedItemInfo.Utils;

public static class ItemUtils {
    /// <summary>
    /// 获取特定ID物品在仓库、玩家与宠物身上的数量
    /// </summary>
    /// <param name="typeID">物品ID</param>
    /// <returns>inStorage在仓库数量；onPlayer在玩家与宠物身上</returns>
    public static (int inStorage, int onPlayer, int onPet) GetItemCount(int typeID) {
        int inStorage = GetItemCountInStorage(typeID);
        int onPlayer = GetItemCountOnPlayer(typeID);
        int onPet = GetItemCountOnPet(typeID);
        return (inStorage, onPlayer, onPet);
    }
    /// <summary>
    /// 获取特定ID物品在仓库的数量 <br/>
    /// 不包括插槽内物品
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetItemCountInStorage(int typeID) {
        Inventory? playerStorage = PlayerStorage.Inventory;
        if (playerStorage == null) {
            Logger.Warn("Null player storage");
            return 0;
        }
        return playerStorage.FindAll(item => item != null && item.TypeID == typeID).Sum(item => item.StackCount);
    }
    /// <summary>
    /// 获取特定ID物品在玩家背包的数量 <br/>
    /// 不包括身上装备和插槽内物品
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetItemCountOnPlayer(int typeID) {
        Inventory? characterInventory = LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory;
        if (characterInventory != null) {
            return characterInventory.FindAll(item => item != null && item.TypeID == typeID).Sum(item => item.StackCount);
        }
        else {
            Logger.Warn("Null player inventory");
#if DEBUG
            if (LevelManager.Instance == null) {
                Logger.Debug("\tNull LevelManager Instance");
            }
            else if (LevelManager.Instance.MainCharacter == null) {
                Logger.Debug("\tNull MainCharacter");
            }
            else if (LevelManager.Instance.MainCharacter.CharacterItem == null) {
                Logger.Debug("\tNull Main Character Item");
            }
#endif
        }
        return 0;
    }
    /// <summary>
    /// 获取特定ID物品在狗子身上的数量
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetItemCountOnPet(int typeID) {
        Inventory? petInventory = LevelManager.Instance?.PetProxy?.Inventory;
        if (petInventory != null) {
            return petInventory.FindAll(item => item != null && item.TypeID == typeID).Sum(item => item.StackCount);
        }
        else {
            Logger.Warn("Null pet inventory");
#if DEBUG
            if (LevelManager.Instance == null) {
                Logger.Debug("\tNull LevelManager Instance");
            }
            else if (LevelManager.Instance.PetProxy == null) {
                Logger.Debug("\tNull PetProxy");
            }
#endif
        }
        return 0;
    }

    public static int GetQuestRequirement(int typeID) {
        if (Patch_QuestManager_SetupSaveData.itemQuestCount.TryGetValue(typeID, out int requirement)) {
            return requirement;
        }
        return 0;
    }
    public static long GetBuildingRequirement(int typeID) {
        if (Patch_BuildingManager.itemBuildingCount.TryGetValue(typeID, out long requirement)) {
            return requirement;
        }
        return 0;
    }
    public static long GetPerkRequirement(int typeID) {
        if (Patch_PerkTree_SetupSaveData.itemPerkCount.TryGetValue(typeID, out long requirement)) {
            return requirement;
        }
        return 0;
    }

    public static Cost.ItemEntry[] GetDecomposeItems(int typeID) {
        var formula = DecomposeDatabase.Instance.GetFormula(typeID);
        if (!formula.valid) {
            return [];
        }
        return formula.result.items;
    }

    public static bool TagContains(IEnumerable<Tag>? tags, string name) {
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && tag.name.Equals(name));
    }
    public static bool IsKey(this Item item) => TagContains(item.Tags, Constant.ItemKeyTag);
    public static bool IsKey(this ItemMetaData itemMetaData) => TagContains(itemMetaData.tags, Constant.ItemKeyTag);
    public static bool IsKeyItem(int typeID) => ItemAssetsCollection.GetMetaData(typeID).IsKey();
    public static bool IsFormula(this Item item) => TagContains(item.Tags, Constant.ItemFormulaTag);
    public static bool IsFormula(this ItemMetaData itemMetaData) => TagContains(itemMetaData.tags, Constant.ItemFormulaTag);
    public static bool IsFormulaItem(int typeID) => ItemAssetsCollection.GetMetaData(typeID).IsFormula();
    public static bool IsKeyOrFormula(this Item item) {
        var tags = item.Tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && (tag.name.Equals(Constant.ItemKeyTag) || tag.name.Equals(Constant.ItemFormulaTag)));
    }
    public static bool IsKeyOrFormula(this ItemMetaData itemMetaData) {
        var tags = itemMetaData.tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && (tag.name.Equals(Constant.ItemKeyTag) || tag.name.Equals(Constant.ItemFormulaTag)));
    }
    public static bool IsKeyOrFormulaItem(int typeID) => ItemAssetsCollection.GetMetaData(typeID).IsKeyOrFormula();
    public static bool IsRegistered(this ItemMetaData itemMetaData) {
        if (!itemMetaData.IsKeyOrFormula()) {
            return false;
        }
        Item? prefab = ItemAssetsCollection.GetPrefab(itemMetaData.id);
        if (prefab == null) {
            return false;
        }
        return prefab.IsRegistered();
    }
    public static bool IsRegisteredItem(int typeID) {
        if (!IsKeyOrFormulaItem(typeID)) {
            return false;
        }
        Item? prefab = ItemAssetsCollection.GetPrefab(typeID);
        if (prefab == null) {
            return false;
        }
        return prefab.IsRegistered();
    }
}
