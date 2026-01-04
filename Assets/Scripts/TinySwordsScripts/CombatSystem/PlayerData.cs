using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    public StatsUI statsUI;

    [Header("Health Stats")]
    public int maxHP = 12;
    public int currentHP = 12;

    [Header("Combat Stats")]
    public int damage = 0;
    public int defense = 0;

    [Header("Movement Stats")]
    public float speed = 3.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHP += amount;
        statsUI.UpdateAllStats();
    }

    public void UpdateHealth(int amount)
    {
        currentHP += amount;
        if (currentHP >= maxHP)
            currentHP = maxHP;
        
        statsUI.UpdateAllStats();

    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI.UpdateAllStats();
    }

    public void UpdateDamage(int amount)
    {
        damage += amount;
        statsUI.UpdateAllStats();
    }
    
    public void UpdateDefense(int amount)
    {
        defense += amount;
        statsUI.UpdateAllStats();
    }
}