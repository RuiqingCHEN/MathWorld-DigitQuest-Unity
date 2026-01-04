using Cinemachine;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;

    private ExpManager expManager;
    private SkillTreeManager skillTreeManager;

    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        // C:\Users\[用户名]\AppData\LocalLow\[公司名]\[产品名]\saveData.json
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = Object.FindFirstObjectByType<InventoryController>();
        hotbarController = Object.FindFirstObjectByType<HotbarController>();
        chests = Object.FindObjectsByType<Chest>(FindObjectsSortMode.None);
        expManager = Object.FindFirstObjectByType<ExpManager>();
        skillTreeManager = Object.FindFirstObjectByType<SkillTreeManager>();
    }
    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = Object.FindFirstObjectByType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
            chestSaveData = GetChestState(),
            questProgressData = QuestController.Instance.activateQuests,
            handinQuestIDs = QuestController.Instance.handinQuestIDs,
            gold = inventoryController.gold,
            playerStats = GetPlayerStats(),
            skillTreeData = GetSkillTreeData()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            };
            chestStates.Add(chestSaveData);
        }
        return chestStates;
    }
    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            PolygonCollider2D savedMapBoundary = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
            Object.FindFirstObjectByType<CinemachineConfiner>().m_BoundingShape2D = savedMapBoundary;

            MapController.Instance?.GenerateMap(savedMapBoundary);

            inventoryController.SetInventoryItem(saveData.inventorySaveData);
            hotbarController.SetHotbarItem(saveData.hotbarSaveData);

            LoadChestStates(saveData.chestSaveData);

            QuestController.Instance.LoadQuestProgress(saveData.questProgressData);
            QuestController.Instance.handinQuestIDs = saveData.handinQuestIDs;

            LoadPlayerStats(saveData.playerStats);
            LoadSkillTreeData(saveData.skillTreeData);

            inventoryController.gold = saveData.gold;
            inventoryController.goldText.text = saveData.gold.ToString();
        }
        else
        {
            SaveGame();
            inventoryController.SetInventoryItem(new List<InventorySaveData>());
            hotbarController.SetHotbarItem(new List<InventorySaveData>());
            MapController.Instance?.GenerateMap();

            inventoryController.gold = 0;
            inventoryController.goldText.text = "0";
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private PlayerStatsSaveData GetPlayerStats()
    {
        return new PlayerStatsSaveData
        {
            currentHP = PlayerData.Instance.currentHP,
            maxHP = PlayerData.Instance.maxHP,
            damage = PlayerData.Instance.damage,
            defense = PlayerData.Instance.defense,
            speed = PlayerData.Instance.speed,
            level = expManager.level,
            currentExp = expManager.currentExp,
            expToLevel = expManager.expToLevel
        };
    }

    private SkillTreeSaveData GetSkillTreeData()
    {
        List<SkillSlotSaveData> skillSlotData = new List<SkillSlotSaveData>();
        foreach (SkillSlot slot in skillTreeManager.skillSlots)
        {
            skillSlotData.Add(new SkillSlotSaveData
            {
                slotID = slot.slotID,
                skillName = slot.skillSO.skillName,
                currentLevel = slot.currentLevel,
                isUnlocked = slot.isUnlocked
            });
        }
        return new SkillTreeSaveData
        {
            availablePoints = skillTreeManager.availablePoints,
            skillSlots = skillSlotData
        };
    }

    private void LoadPlayerStats(PlayerStatsSaveData data)
    {
        if (data == null) return;
        PlayerData.Instance.currentHP = data.currentHP;
        PlayerData.Instance.maxHP = data.maxHP;
        PlayerData.Instance.damage = data.damage;
        PlayerData.Instance.defense = data.defense;
        PlayerData.Instance.speed = data.speed;
        expManager.level = data.level;
        expManager.currentExp = data.currentExp;
        expManager.expToLevel = data.expToLevel;
        expManager.UpdateUI();
        PlayerData.Instance.statsUI.UpdateAllStats();
    }

    private void LoadSkillTreeData(SkillTreeSaveData data)
    {
        if (data == null) return;
        skillTreeManager.availablePoints = data.availablePoints;
        skillTreeManager.UpdateAbilityPoints(0);
        foreach (SkillSlotSaveData slotData in data.skillSlots)
        {
            SkillSlot slot = System.Array.Find(skillTreeManager.skillSlots,
                s => s.slotID == slotData.slotID);
            if (slot != null)
            {
                slot.currentLevel = slotData.currentLevel;
                slot.isUnlocked = slotData.isUnlocked;
                slot.UpdateUI();
            }
        }

        foreach (SkillSlot slot in skillTreeManager.skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
            {
                slot.Unlock();
            }
        }
    }
}
