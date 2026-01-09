using Duckov.Quests;
using Duckov.Quests.Tasks;
using EnhancedItemInfo.Attributes;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System.Collections.Generic;

namespace EnhancedItemInfo.Patches;

[Patch]
[HarmonyPatch(typeof(QuestManager), nameof(QuestManager.SetupSaveData))]
internal class Patch_QuestManager_SetupSaveData {
    // 每种物品的任务需求（active + everInspected）
    public static readonly Dictionary<int, int> itemQuestCount = [];
    // 记录的每个任务的剩余需求数量 (使用/上交物品)
    static readonly Dictionary<int, int> taskRemainedAmount = [];

    static void UpdateStats(int taskID, int itemTypeID, int preRemainedAmount, int newRemainedAmount) {
        int changedAmount = preRemainedAmount - newRemainedAmount;
        if (changedAmount != 0) {
            if (itemQuestCount.TryGetValue(itemTypeID, out int preValue)) {
                itemQuestCount[itemTypeID] = preValue - changedAmount;
            }
            else {
                itemQuestCount[itemTypeID] = -changedAmount;
            }
            taskRemainedAmount[taskID] = newRemainedAmount;
        }
    }
    static void HandleUseItemTask(QuestTask_UseItem task) {
        var taskInstance = Traverse.Create(task);
        int itemTypeID = taskInstance.Field("itemTypeID").GetValue<int>();
        int amount = taskInstance.Field("amount").GetValue<int>();
        int requireAmount = taskInstance.Field("requireAmount").GetValue<int>();
        taskRemainedAmount.TryGetValue(task.ID, out int remainedAmount);
        if (task.IsFinished()) {
            if (remainedAmount != 0) {
                itemQuestCount[itemTypeID] -= remainedAmount;
            }
            return;
        }
        UpdateStats(task.ID, itemTypeID, remainedAmount, requireAmount - amount);
        task.onStatusChanged += OnTaskUpdated;
    }
    static void HandleSubmitItemTask(SubmitItems task) {
        var taskInstance = Traverse.Create(task);
        int itemTypeID = taskInstance.Field("itemTypeID").GetValue<int>();
        int amount = taskInstance.Field("submittedAmount").GetValue<int>();
        int requireAmount = taskInstance.Field("requireAmount").GetValue<int>();
        taskRemainedAmount.TryGetValue(task.ID, out int remainedAmount);
        if (task.IsFinished()) {
            if (remainedAmount != 0) {
                itemQuestCount[itemTypeID] -= remainedAmount;
            }
            return;
        }
        UpdateStats(task.ID, itemTypeID, remainedAmount, requireAmount - amount);
        task.onStatusChanged += OnTaskUpdated;
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
        foreach (Task? task in quest.Tasks) {
            if (task == null || task.IsFinished()) {
                continue;
            }
            HandleTask(task);
        }
    }

    static void Postfix(QuestManager __instance) {
        Logger.Info("Init quest data");
        itemQuestCount.Clear();
        taskRemainedAmount.Clear();

        foreach (Quest? quest in __instance.ActiveQuests) {
            if (quest == null) {
                Logger.Warn($"Null ActiveQuest meet");
                continue;
            }
            HandleQuest(quest);
        }
        foreach (int questID in __instance.EverInspectedQuest) {
            Quest? quest = QuestCollection.Instance?.Get(questID);
            if (quest == null) {
                Logger.Warn($"Null EverInspectedQuest {questID}");
                continue;
            }
            HandleQuest(quest);
        }
    }
}
