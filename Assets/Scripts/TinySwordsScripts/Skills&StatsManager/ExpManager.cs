using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f; // Add 20% more EXP to level

    public Slider expSlider;
    public TMP_Text currentLevelText;

    public static event Action<int> OnLevelUp;

    private void Start()
    {
        UpdateUI();
    }
    /* Observer Pattern
    敌人/人物系统：
    public int expReward = 3;
    public delegate void MonsterDefeated(int exp);
    定义了一个 委托类型 (delegate type)，名字叫 MonsterDefeated，规定了事件触发时调用的函数必须是 void 函数(int 参数)
    public static event MonsterDefeated OnMonsterDefeated;
    这行定义了一个 事件 (event)，类型是前面声明的 MonsterDefeated，把 expReward 这个参数传递给所有订阅了这个事件的函数

    用法(在需要获得经验的地方): OnMonsterDefeated(expReward);
    */
    private void OnEnable()
    {
        BattleSystem.OnBattleWon += GainExperience;
    }
    private void OnDisable()
    {
        BattleSystem.OnBattleWon -= GainExperience;
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        OnLevelUp?.Invoke(1); // 升一级给1个技能点数
    }

    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        currentLevelText.text = "Level: " + level;
    }
}
