using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjective> objectives;
    public List<QuestReward> questRewards;

    // Called when scriptable obj is edited
    // OnValidate() 是一个特殊的 MonoBehaviour 方法，只要你在编辑器里改了 Inspector 面板上的值，它就会自动运行，用来做数据修正或自动初始化。
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            // 生成一个 全局唯一标识符（Globally Unique Identifier，简称 GUID）
            questID = questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID; // Match with item ID that you need to collect, enemy ID that you need to kill etc
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount;

}

public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkNPC, Custom }

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        // Deep copy avoid modifying original
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);

    public string QuestID => quest.questID;
}

[System.Serializable]
public class QuestReward
{
    public RewardType type;
    public string rewardID; // ItemID etc
    public int amount = 1;
}

public enum RewardType { Item, Gold, Experience, Custom }