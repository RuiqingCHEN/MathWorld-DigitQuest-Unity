using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartContainer; 
    public List<Sprite> heartSprites;
    
    private List<Image> heartImages = new List<Image>();
    
    private int healthPerHeart = 4;
    private int lastHealthValue;
    private int lastMaxHealth;
    
    void Update()
    {
        if (PlayerData.Instance != null)
        {
            int currentHP = PlayerData.Instance.currentHP;
            int maxHP = PlayerData.Instance.maxHP;

            if (maxHP != lastMaxHealth)
            {
                GenerateHearts(maxHP);
                lastMaxHealth = maxHP;
                lastHealthValue = -1;
            }

            if (currentHP != lastHealthValue)
            {
                UpdateHealthUI();
                lastHealthValue = currentHP;
            }
        }
    }
    
    void GenerateHearts(int maxHP)
    {
        foreach (Image heart in heartImages)
        {
            if (heart != null)
                Destroy(heart.gameObject);
        }
        heartImages.Clear();
        
        int heartsNeeded = Mathf.CeilToInt((float)maxHP / healthPerHeart);
        
        for (int i = 0; i < heartsNeeded; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, heartContainer);
            Image heartImage = heartObj.GetComponent<Image>();
            
            if (heartImage != null)
            {
                heartImages.Add(heartImage);
            }
        }
    }

    void UpdateHealthUI()
    {
        if (PlayerData.Instance == null) return;

        int currentHealth = PlayerData.Instance.currentHP;
        
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (heartImages[i] == null) continue;
            
            int heartMinHealth = i * healthPerHeart;
            int heartMaxHealth = heartMinHealth + healthPerHeart;
            
            if (currentHealth >= heartMaxHealth)
            {
                heartImages[i].sprite = heartSprites[4];
            }
            else if (currentHealth > heartMinHealth)
            {
                int healthInHeart = currentHealth - heartMinHealth;
                heartImages[i].sprite = heartSprites[healthInHeart];
            }
            else
            {
                heartImages[i].sprite = heartSprites[0];
            }
        }
    }

}
