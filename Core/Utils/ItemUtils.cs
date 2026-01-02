using Duckov.Economy;
using Duckov.Utilities;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedItemInfo.Core.Utils;

public static class ItemUtils {
    /// <summary>
    /// 获取特定ID物品在仓库、玩家与宠物身上的数量
    /// </summary>
    /// <param name="typeID">物品ID</param>
    /// <returns>inStorage在仓库数量；onPlayer在玩家与宠物身上</returns>
    public static (int inStorage, int onPlayer) GetItemCount(int typeID) {
        int inStorage = GetItemCountInStorage(typeID);
        int onPlayer = GetItemCountOnPlayer(typeID);
        return (inStorage, onPlayer);
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
    /// 获取特定ID物品在玩家与宠物身上的数量 <br/>
    /// 不包括身上装备和插槽内物品
    /// </summary>
    /// <param name="typeID">物品ID</param>
    public static int GetItemCountOnPlayer(int typeID) {
        int count = 0;
        Inventory? characterInventory = LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory;
        if (characterInventory != null) {
            count += characterInventory.FindAll(item => item != null && item.TypeID == typeID).Sum(item => item.StackCount);
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
        Inventory? petInventory = LevelManager.Instance?.PetProxy?.Inventory;
        if (petInventory != null) {
            count += petInventory.FindAll(item => item != null && item.TypeID == typeID).Sum(item => item.StackCount);
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
        return count;
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
    public static bool IsKey(this Item item) => TagContains(item.Tags, Constant.KeyTag);
    public static bool IsKey(this ItemMetaData itemMetaData) => TagContains(itemMetaData.tags, Constant.KeyTag);
    public static bool IsKeyItem(int typeID) => ItemAssetsCollection.GetMetaData(typeID).IsKey();
    public static bool IsFormula(this Item item) => TagContains(item.Tags, Constant.FormulaTag);
    public static bool IsFormula(this ItemMetaData itemMetaData) => TagContains(itemMetaData.tags, Constant.FormulaTag);
    public static bool IsFormulaItem(int typeID) => ItemAssetsCollection.GetMetaData(typeID).IsFormula();
    public static bool IsKeyOrFormula(this Item item) {
        var tags = item.Tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && (tag.name.Equals(Constant.KeyTag) || tag.name.Equals(Constant.FormulaTag)));
    }
    public static bool IsKeyOrFormula(this ItemMetaData itemMetaData) {
        var tags = itemMetaData.tags;
        if (tags == null) {
            return false;
        }
        return tags.Any(tag => tag != null && (tag.name.Equals(Constant.KeyTag) || tag.name.Equals(Constant.FormulaTag)));
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
