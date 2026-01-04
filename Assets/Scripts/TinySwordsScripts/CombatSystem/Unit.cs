using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    [Header("Components")]
    public Image spriteRenderer;

    [Header("Unit Stats")]
    public string unitName;
    public int unitLevel;
    public int damage;
    public int maxHP;
    public int currentHP;

    private Sprite deadSprite;
    private EnemyData enemyData;

    public void Initialize(string name, int level, int maxHp, int dmg, Sprite aliveSprite, Sprite deathSprite)
    {
        unitName = name;
        unitLevel = level;
        maxHP = maxHp;
        damage = dmg;
        currentHP = maxHP;

        deadSprite = deathSprite;

        if (spriteRenderer != null && aliveSprite != null)
        {
            spriteRenderer.sprite = aliveSprite;
        }

    }
    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            ShowDeathSprite();
            return true;
        }
        return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    private void ShowDeathSprite()
    {
        if (spriteRenderer != null && deadSprite != null)
        {
            spriteRenderer.sprite = deadSprite;
        }
    }
    

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }
}
