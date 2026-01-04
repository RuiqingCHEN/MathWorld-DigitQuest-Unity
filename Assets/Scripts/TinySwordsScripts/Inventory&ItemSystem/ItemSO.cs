using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public bool isGold;
    public int stackSize = 3;

    [Header("Stats")]
    public int currentHP;
    public int maxHP;
    public int speed;
    public int damage;
    public int defense;

    [Header("For Temporary Items")]
    public float duration;



}
