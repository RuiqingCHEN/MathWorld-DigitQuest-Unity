using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary; // The boundary name for the map
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<QuestProgress> questProgressData;
    public List<String> handinQuestIDs;
    public int gold;
    public PlayerStatsSaveData playerStats;
    public SkillTreeSaveData skillTreeData;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}

[System.Serializable]
public class PlayerStatsSaveData
{
    public int currentHP;
    public int maxHP;
    public int damage;
    public int defense;
    public float speed;
    public int level;
    public int currentExp;
    public int expToLevel;
}

[System.Serializable]
public class SkillTreeSaveData
{
    public int availablePoints;
    public List<SkillSlotSaveData> skillSlots;
}

[System.Serializable]
public class SkillSlotSaveData
{
    public string slotID;
    public string skillName;
    public int currentLevel;
    public bool isUnlocked;
}