using Duckov;
using Duckov.PerkTrees;
using Duckov.Quests;
using Duckov.Quests.Conditions;
using Duckov.Quests.Relations;
using Duckov.Quests.Tasks;
using Duckov.Utilities;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Extensions;
using HarmonyLib;
using ItemStatsSystem;
using NodeCanvas.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;
using Task = Duckov.Quests.Task;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(QuestManager), nameof(QuestManager.SetupSaveData))]
internal static class Patch_QuestManager_SetupSaveData {
    public static bool Inited = false;
    static readonly HashSet<int> availableQuests = [];

    static readonly Dictionary<int, bool> checkedQuest = [];
    public static bool CheckQuest(int questID) {
        if (checkedQuest.TryGetValue(questID, out var result)) {
            return result;
        }
        if (QuestCollection.Instance == null) {
            Logger.Warn($"Null QuestCollection when check quest");
            return true;
        }
        var quest = QuestCollection.Instance?.Get(questID);
        if (quest == null) {
            checkedQuest[questID] = false;
            return false;
        }
        if (quest.LockInDemo) {
            if (GameMetaData.Instance.IsDemo) {
                checkedQuest[questID] = false;
                return false;
            }
        }
        Logger.Debug($"\t\tRequire Level {quest.RequireLevel}");
        if (quest.RequireLevel > Constant.MaxLevel) {
            checkedQuest[questID] = false;
            return false;
        }
        Logger.Debug($"\t\tRequire Scene {quest.RequireSceneInfo?.DisplayName}");

        foreach (var condition in quest.Prerequisits) {
            switch (condition) {
                case RequireDemo demo: {
                    Logger.Debug($"\t\tRequire Demo {demo}");
                    if (!demo.Evaluate()) {
                        checkedQuest[questID] = false;
                        return false;
                    }
                    break;
                }
                case RequireFormulaUnlocked formula: {
                    var root = Traverse.Create(formula);
                    int itemID = root.Field("itemID").GetValue<int>();
                    string formulaID = root.Field("formulaID").GetValue<string>();
                    Logger.Debug($"\t\tRequire Formula {itemID} {formulaID}");
                    break;
                }
                case RequireGameobjectsActived gameobjectsActived: {
                    var gameObjects = Traverse.Create(gameobjectsActived).Field("targets").GetValue<GameObject[]>();
                    if (gameObjects != null) {
                        Logger.Debug($"\t\tRequire GameObjects");
                        foreach (var gameObject in gameObjects) {
                            if (gameObject == null && !gameObject) {
                                checkedQuest[questID] = false;
                                return false;
                            }
                        }
                    }
                    break;
                }
                case RequirePerkUnlocked perkUnlocked: {
                    Logger.Debug($"\t\tRequire Perk");
                    perkUnlocked.Evaluate();
                    if (Traverse.Create(perkUnlocked).Field("perk").GetValue<Perk>() == null) {
                        checkedQuest[questID] = false;
                        return false;
                    }
                    break;
                }
                case RequireQuestsActive questsActive: {
                    Logger.Debug($"\t\tRequire Active Quests : {questsActive.RequiredQuestIDs}");
                    foreach (var rquest in questsActive.RequiredQuestIDs) {
                        if (!CheckQuest(rquest)) {
                            checkedQuest[questID] = false;
                            return false;
                        }
                        if (!availableQuests.Contains(rquest)) {
                            Logger.Debug($"\t\t Failed on Quest {rquest}");
                            checkedQuest[questID] = false;
                            return false;
                        }
                    }
                    break;
                }
                case RequireQuestsFinished questsFinished: {
                    Logger.Debug($"\t\tRequire Active Quests : {questsFinished.RequiredQuestIDs}");
                    foreach (var rquest in questsFinished.RequiredQuestIDs) {
                        if (!CheckQuest(rquest)) {
                            checkedQuest[questID] = false;
                            return false;
                        }
                        if (!availableQuests.Contains(rquest)) {
                            Logger.Debug($"\t\t Failed on Quest {rquest}");
                            checkedQuest[questID] = false;
                            return false;
                        }
                    }
                    break;
                }
            }
        }
        checkedQuest[questID] = true;
        return true;
    }
    public static void Init() {
        if (Inited) {
            return;
        }
        Logger.Debug($"Search Quest Relation");
        availableQuests.Clear();
        var graph = GameplayDataSettings.QuestRelation;
        // 拓扑遍历
        Queue<Node> queue = [];
        Dictionary<int, int> inCount = [];
        Dictionary<int, List<Node>> proxyCache = [];
        graph.allNodes.ForEach(node => {
            if (node.inConnections.Count == 0) {
                queue.Enqueue(node);
            }
            inCount.Add(node.ID, node.inConnections.Count);
        });
        while (queue.Count > 0) {
            var node = queue.Dequeue();
            Logger.Debug($"Now Node {node.ID} {string.Join("\t", node.name.Split('\n'))}");
            switch (node) {
                case QuestRelationNode questNode: {
                    Logger.Debug($"\tQuest {questNode.questID}");
                    if (CheckQuest(questNode.questID)) {
                        availableQuests.Add(questNode.questID);
                        if (proxyCache.TryGetValue(questNode.questID, out var lst)) {
                            lst.Do(queue.Enqueue);
                            proxyCache.Remove(questNode.questID);
                        }
                    }
                    else {
                        Logger.Debug($"\t\tCheck Failed");
                        continue;
                    }
                    break;
                }
                case QuestRelationProxyNode questProxyNode: {
                    Logger.Debug($"\tQuest Proxy {questProxyNode.questID}");
                    if (!availableQuests.Contains(questProxyNode.questID)) {
                        proxyCache.GetOrCreate(questProxyNode.questID).Add(node);
                        continue;
                    }
                    break;
                }
            }
            node.GetChildNodes().Do(child => {
                if (!inCount.TryGetValue(child.ID, out int count)) {
                    return;
                }
                Logger.Debug($"\t\t Child Node {child.ID} {string.Join("\t", child.name.Split('\n'))}");
                if (count == 1) {
                    queue.Enqueue(child);
                    inCount.Remove(child.ID);
                }
                else {
                    inCount[child.ID] = count - 1;
                }
            });
        }
        foreach (var (questID, proxies) in proxyCache) {
            if (availableQuests.Contains(questID)) {

            }
        }
        Inited = true;
    }

    // 每种物品的任务需求（active + everInspected）
    // ItemTypeID => (Quest.ID => amount)
    static readonly Dictionary<int, Dictionary<int, int>> itemQuestCount = [];
    // 记录的每个任务的剩余需求数量 (使用/上交物品)
    // (Quest.ID, Task.ID) => amount 注意不同Quest内的Task.ID可能重复
    static readonly Dictionary<(int, int), int> taskRemainedAmount = [];
    // Quest.ID => Quest.DisplayName
    static readonly Dictionary<int, string> QuestName = [];

    public static IEnumerable<(string Name, int Amount)> GetQuestRequirement(int typeID) {
        if (itemQuestCount.TryGetValue(typeID, out var dict)) {
            return dict.AsEnumerable().Select(kv => (Name: QuestName[kv.Key], Amount: kv.Value));
        }
        return [];
    }

    static void UpdateStats(Task task, int itemTypeID, int preRemainedAmount, int newRemainedAmount) {
        int changedAmount = preRemainedAmount - newRemainedAmount;
        if (changedAmount != 0) {
            var dict = itemQuestCount.GetOrCreate(itemTypeID);
            var questID = task.Master.ID;
            if (dict.TryGetValue(questID, out int preValue)) {
                dict[questID] = preValue - changedAmount;
            }
            else {
                dict[questID] = -changedAmount;
            }
            taskRemainedAmount[(questID, task.ID)] = newRemainedAmount;
        }
    }
    static void HandleUseItemTask(QuestTask_UseItem task) {
        Logger.Debug($"\t\tHandle Use {task.ID}");
        var taskInstance = Traverse.Create(task);
        int itemTypeID = taskInstance.Field("itemTypeID").GetValue<int>();
        int amount = taskInstance.Field("amount").GetValue<int>();
        int requireAmount = taskInstance.Field("requireAmount").GetValue<int>();
#if DEBUG
        var item = ItemAssetsCollection.GetMetaData(itemTypeID);
        Logger.Debug($"\t\t\t{itemTypeID} {item.DisplayName} {amount}/{requireAmount}");
#endif
        taskRemainedAmount.TryGetValue((task.Master.ID, task.ID), out int remainedAmount);
        if (task.IsFinished()) {
            amount = requireAmount; // forceFinish
        }
        else {
            task.onStatusChanged += OnTaskUpdated;
        }
        UpdateStats(task, itemTypeID, remainedAmount, requireAmount - amount);
    }
    static void HandleSubmitItemTask(SubmitItems task) {
        Logger.Debug($"\t\tHandle Submit {task.ID}");
        var taskInstance = Traverse.Create(task);
        int itemTypeID = taskInstance.Field("itemTypeID").GetValue<int>();
        int amount = taskInstance.Field("submittedAmount").GetValue<int>();
        int requireAmount = taskInstance.Field("requiredAmount").GetValue<int>();
#if DEBUG
        var item = ItemAssetsCollection.GetMetaData(itemTypeID);
        Logger.Debug($"\t\t\t{itemTypeID} {item.DisplayName} {amount}/{requireAmount}");
#endif
        taskRemainedAmount.TryGetValue((task.Master.ID, task.ID), out int remainedAmount);
        if (task.IsFinished()) {
            amount = requireAmount; // forceFinish
        }
        else {
            task.onStatusChanged += OnTaskUpdated;
        }
        UpdateStats(task, itemTypeID, remainedAmount, requireAmount - amount);
    }
    static void HandleTask(Task task) {
        switch (task) {
            case QuestTask_UseItem useItem: {
                HandleUseItemTask(useItem);
                break;
            }
            case SubmitItems submitItems: {
                HandleSubmitItemTask(submitItems);
                break;
            }
        }
    }
    static void OnTaskUpdated(Task task) {
        task.onStatusChanged -= OnTaskUpdated;
        HandleTask(task);
    }
    static void HandleQuest(Quest quest) {
        if (!availableQuests.Contains(quest.ID)) {
            Logger.Debug($"Quest not available {quest.ID} {quest.QuestGiverID} [{quest.DisplayName}]");
            return;
        }
        if (QuestName.ContainsKey(quest.ID)) {
            Logger.Warn($"Duplicated Quest {quest.ID} {QuestName[quest.ID]} ? [{quest.DisplayName}]");
            return;
        }
        Logger.Debug($"\tHandle Quest {quest.ID} {quest.QuestGiverID} [{quest.DisplayName}]");
        if (string.IsNullOrEmpty(quest.DisplayName)) {
            Logger.Warn($"\t\tNull DisplayName!");
        }
        QuestName.Add(quest.ID, quest.DisplayName);
        foreach (Task? task in quest.Tasks) {
            if (task == null || task.IsFinished()) {
                continue;
            }
            HandleTask(task);
        }
    }

    static void Postfix(QuestManager __instance) {
        Init();
        Logger.Info("Init quest data");
        itemQuestCount.Clear();
        taskRemainedAmount.Clear();
        QuestName.Clear();

        Logger.Debug($"Record History Quest");
        foreach (var quest in __instance.HistoryQuests) {
            if (quest == null) {
                Logger.Warn($"Null History Quest meet");
                continue;
            }
            Logger.Debug($"\t{quest.ID} {quest.DisplayName}");
            QuestName.Add(quest.ID, quest.DisplayName);
        }
        Logger.Debug($"Handle Active Quest");
        foreach (Quest? quest in __instance.ActiveQuests) {
            if (quest == null) {
                Logger.Warn($"Null Active Quest meet");
                continue;
            }
            HandleQuest(quest);
        }
        Logger.Debug($"Handle Quest Collection");
        var questCollection = QuestCollection.Instance;
        if (questCollection == null) {
            Logger.Warn($"Null QuestCollection");
            return;
        }
        foreach (var quest in questCollection) {
            if (quest == null) {
                Logger.Debug($"Null Quest in Collection");
                continue;
            }
            if (QuestName.ContainsKey(quest.ID)) {
                continue;
            }
            HandleQuest(quest);
        }
    }
}
