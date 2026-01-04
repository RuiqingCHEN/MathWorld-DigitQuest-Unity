using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryController inventoryController;

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if (itemSO != null && inventoryController.gold >= price)
        {
            if (HasSpaceForItem(itemSO))
            {
                inventoryController.gold -= price;
                inventoryController.goldText.text = inventoryController.gold.ToString();
                bool itemAdded = inventoryController.AddItem(itemSO, 1);
                
                if (itemAdded && ItemPickupUIController.Instance != null)
                {
                    ItemPickupUIController.Instance.ShowItemPickup(itemSO.itemName, itemSO.icon);
                }
            }
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                // Found a slot with the same item that has space for more
                return true;
            }
        }

        // Check for empty slots
        InventorySlot emptySlot = inventoryController.FindEmptySlot();
        return emptySlot != null;
    }

    public void SellItem(ItemSO itemSO)
    {
        if (itemSO == null)
            return;
        foreach (var slot in shopSlots)
        {
            if (slot.itemSO == itemSO)
            {
                inventoryController.gold += slot.price;
                inventoryController.goldText.text = inventoryController.gold.ToString();
                return;
            }
        }
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}