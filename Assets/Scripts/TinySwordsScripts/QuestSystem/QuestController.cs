using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;

    public List<string> handinQuestIDs = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindFirstObjectByType<QuestUI>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.questID)) return;
        // 不接受已经存在的新任务
        activateQuests.Add(new QuestProgress(quest));
        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }

    // 这个方法用于检查指定ID的任务是否在活跃任务列表中
    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID);

    public void CheckInventoryForQuests()
    {
        Dictionary<string, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach (QuestProgress quest in activateQuests)
        {
            foreach (QuestObjective questObjective in quest.objectives)
            {
                if (questObjective.type != ObjectiveType.CollectItem) continue;
                
                string itemName = questObjective.objectiveID;
                int newAmount = itemCounts.TryGetValue(itemName, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;

                if (questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }

        questUI.UpdateQuestUI();
    }

    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        // Try remove required items
        if (!RemoveRequiredItemsFromInventory(questID))
        {
            // Quest couldn't be completed - missing item
            return;
        }
        // Remove quest from quest log
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest != null)
        {
            handinQuestIDs.Add(questID);
            activateQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }

    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;

        Dictionary<string, int> requiredItems = new();
        // Item requirements from objectives
        foreach (QuestObjective objective in quest.objectives)
        {
            // int.TryParse() 是C#的静态方法，用于安全地将字符串转换为整数
            if (objective.type == ObjectiveType.CollectItem)
            {
                requiredItems[objective.objectiveID] = objective.requiredAmount; // Potion 5/5 - 1,5
            }
        }

        // Verify we have items
        Dictionary<string, int> itemCounts = InventoryController.Instance.GetItemCounts();
        // var 告诉编译器："请你根据右边的值自动推断这个变量的类型"
        foreach (var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                // Not enough items to complete quest
                return false;
            }
        }

        // Remove required items from inventory
        foreach (var itemRequirement in requiredItems)
        {
            // RemoveItemsFromInventory
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }

    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activateQuests = savedQuests ?? new();
        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }
}
