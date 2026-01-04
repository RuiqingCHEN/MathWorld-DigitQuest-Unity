using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<ItemSO> itemSOs;
    private Dictionary<string, ItemSO> itemDictionary;
    
    private void Awake()
    {
        itemDictionary = new Dictionary<string, ItemSO>();

        foreach(ItemSO itemSO in itemSOs)
        {
            if(itemSO != null)
            {
                itemDictionary[itemSO.itemName] = itemSO;
            }
        }
    }

    public ItemSO GetItemPrefab(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("ItemName is null or empty!");
            return null;
        }
        
        itemDictionary.TryGetValue(itemName, out ItemSO itemSO);
        if(itemSO == null)
        {
            Debug.LogWarning($"ItemSO with name '{itemName}' not found in dictionary");
        }
        return itemSO;
    }
}