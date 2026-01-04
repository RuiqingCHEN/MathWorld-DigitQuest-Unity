using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text[] statsSlots;

    private void Start()
    {
        UpdateAllStats();
    }
    private void Update()
    {
        UpdateAllStats();
    }
    public void UpdateHealth()
    {
        statsSlots[0].text = "HP: " + PlayerData.Instance.currentHP + " / " + PlayerData.Instance.maxHP;
        healthSlider.maxValue = PlayerData.Instance.maxHP;
        healthSlider.value = PlayerData.Instance.currentHP;
        statsSlots[1].text = "MAX HP: " + PlayerData.Instance.maxHP;
    }

    public void UpdateDamage()
    {
        statsSlots[2].text = "DAMAGE: " + PlayerData.Instance.damage;
    }

    public void UpdateDefense()
    {
        statsSlots[3].text = "DEFENSE: " + PlayerData.Instance.defense;
    }

    public void UpdateSpeed()
    {
        statsSlots[4].text = "SPEED: " + PlayerData.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateHealth();
        UpdateDamage();
        UpdateDefense();
        UpdateSpeed();
    }
}
