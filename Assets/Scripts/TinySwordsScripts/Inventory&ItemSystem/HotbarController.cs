using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10; // 1-0 on the keyboard

    private ItemDictionary itemDictionary;
    private UseItem useItem;
    private Key[] hotbarKeys;
    
    private void Awake()
    {
        itemDictionary = Object.FindFirstObjectByType<ItemDictionary>();
        useItem = Object.FindFirstObjectByType<UseItem>();

        hotbarKeys = new Key[slotCount];
        for(int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    void Update()
    {
        // Check for key presses
        for(int i = 0; i < slotCount; i++)
        {
            if(Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                // Use Item
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
        if (index >= hotbarPanel.transform.childCount) return;
        
        InventorySlot slot = hotbarPanel.transform.GetChild(index).GetComponent<InventorySlot>();
        if(slot != null && slot.itemSO != null && slot.quantity > 0)
        {
            if (useItem != null)
            {
                useItem.ApplyItemEffects(slot.itemSO);
                slot.quantity--;
                if (slot.quantity <= 0)
                {
                    slot.itemSO = null;
                    slot.quantity = 0;
                }
                slot.UpdateUI();
            }
        }
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO != null && slot.quantity > 0)
            {
                hotbarData.Add(new InventorySaveData 
                { 
                    itemName = slot.itemSO.itemName,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = slot.quantity
                });
            }
        }
        return hotbarData;
    }


    public void SetHotbarItem(List<InventorySaveData> hotbarSaveData)
    {
        // Clear inventory panel - avoid duplicates
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, hotbarPanel.transform);

            TMP_Text key = slot.transform.Find("Key").GetComponent<TMP_Text>();
            if (key != null)
            {
                key.text = i < 9 ? (i + 1).ToString() : "0";
            }
        }

        // Populate slots with saved items
        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                InventorySlot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<InventorySlot>();
                ItemSO itemSO = itemDictionary?.GetItemPrefab(data.itemName);
                
                if (itemSO != null)
                {
                    slot.itemSO = itemSO;
                    slot.quantity = data.quantity;
                    slot.UpdateUI();
                }
            }
        }
    }
}
