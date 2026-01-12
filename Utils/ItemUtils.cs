using Duckov.Economy;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Patches;
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
    /// 不包括插槽内物品 <br/>
    /// 这个方法在基地外会返回缓存数量
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetItemCountInStorage(int typeID) {
        if (Patches_PlayStorage.Cache) {
            if (Patches_PlayStorage.ItemCountCache.TryGetValue(typeID, out int count)) {
                return count;
            }
            return 0;
        }
        return GetCurrentItemCountInStorage(typeID);
    }
    /// <summary>
    /// 获取特定ID物品在仓库的数量 <br/>
    /// 不包括插槽内物品 <br/>
    /// 这个方法不会使用缓存，在基地外会返回0
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetCurrentItemCountInStorage(int typeID) {
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
        if (Patches_BuildingManager.itemBuildingCount.TryGetValue(typeID, out long requirement)) {
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
        return Patch_DecomposeDatabase_RebuildDictionary.GetFormulaByTypeID(typeID);
    }
    public static HashSet<(int, long)> GetDecomposeFromItems(int typeID) {
        return Patch_DecomposeDatabase_RebuildDictionary.GetDecomposeFromByTypeID(typeID);
    }

    public static bool IsKeyOrFormula(this ItemMetaData itemMetaData) {
        var tags = itemMetaData.tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && (tag.name.Equals(Constant.ItemKeyTag) || tag.name.Equals(Constant.ItemFormulaTag)));
    }
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
}
