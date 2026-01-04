using UnityEngine;

[CreateAssetMenu(menuName = "Battle/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public int level;
    
    [Header("Sprites")]
    public Sprite aliveSprite;
    public Sprite deadSprite;
    
    [Header("Stats")]
    public int maxHP;
    public int damage;
    
    [Header("Quiz Settings")] 
    public QuestionConfig questionConfig;

    [Header("Battle Rewards")]
    public int expReward = 5;
    public BattleReward[] battleRewards;
}

[System.Serializable]
public class BattleReward
{
    public string rewardID;
    public int amount = 1;
}

[System.Serializable]
public class NPCBattleConfig
{
    public EnemyData enemyData;
}