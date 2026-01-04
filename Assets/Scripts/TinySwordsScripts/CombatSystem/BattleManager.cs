using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    
    public GameObject battlePanel;  // 战斗UI面板
    
    // 存储战斗相关数据
    private static NPCBattleConfig currentBattleConfig;
    private static bool battleWon = false;
    private static NPC battleNPC; // 记住触发战斗的NPC
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (battlePanel != null)
        {
            battlePanel.SetActive(false);
        }
    }
    
    // 开始战斗
    public void StartBattle(NPCBattleConfig battleConfig, NPC npc = null)
    {
        currentBattleConfig = battleConfig;
        battleWon = false;
        battleNPC = npc; 
        
        Debug.Log($"Starting battle against {battleConfig.enemyData.enemyName}");

        PauseController.SetPause(true);
        
        // 显示战斗UI面板
        if (battlePanel != null)
        {
            battlePanel.SetActive(true);
        }
    }
    
    // 战斗结束
    public void EndBattle(bool playerWon)
    {
        battleWon = playerWon;
        Debug.Log($"Battle ended. Player won: {playerWon}");
        
        // 隐藏战斗UI面板
        if (battlePanel != null)
        {
            battlePanel.SetActive(false);
        }
        
        if (battleNPC != null)
        {
            battleNPC.Interact();
            battleNPC = null;
        }
    }
    
    // 清除战斗结果（在显示完战斗结果对话后调用）
    public static void ClearBattleResult()
    {
        battleWon = false;
        currentBattleConfig = null;
    }
    
    public static NPCBattleConfig GetCurrentBattleConfig()
    {
        return currentBattleConfig;
    }
    
    public static bool GetBattleResult()
    {
        return battleWon;
    }
}