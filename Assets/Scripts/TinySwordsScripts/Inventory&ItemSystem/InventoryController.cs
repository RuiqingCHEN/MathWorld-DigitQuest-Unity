using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public GameObject lootPrefab;
    public int slotCount;

    public static InventoryController Instance { get; private set; }
    Dictionary<string, int> itemsCountCache = new();
    public event Action OnInventoryChanged; // event to notify quest system (or any other system that needs to know!)

    public int gold = 0;
    public TMP_Text goldText;

    private void OnEnable()
    {
        Loot.OnItemLooted += OnItemLooted;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= OnItemLooted;
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        RebuildItemCounts();
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO != null && slot.quantity > 0)
            {
                string itemName = slot.itemSO.itemName;
                itemsCountCache[itemName] = itemsCountCache.GetValueOrDefault(itemName, 0) + slot.quantity;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public Dictionary<string, int> GetItemCounts() => itemsCountCache;

    public bool AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO == null) return false;
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return true;
        }
        int remainingQuantity = quantity;

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, remainingQuantity);

                slot.quantity += amountToAdd;
                remainingQuantity -= amountToAdd;
                slot.UpdateUI();

                if (remainingQuantity <= 0)
                {
                    RebuildItemCounts();
                    return true;
                }
            }
        }

        while (remainingQuantity > 0)
        {
            InventorySlot emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            int amountToAdd = Mathf.Min(itemSO.stackSize, remainingQuantity);
            emptySlot.itemSO = itemSO;
            emptySlot.quantity = amountToAdd;
            emptySlot.UpdateUI();

            remainingQuantity -= amountToAdd;
        }

        RebuildItemCounts();
        return true;
    }

    public InventorySlot FindEmptySlot()
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO == null)
            {
                return slot;
            }
        }
        return null;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO != null && slot.quantity > 0)
            {
                invData.Add(new InventorySaveData
                {
                    itemName = slot.itemSO.itemName,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = slot.quantity
                });
            }
        }
        return invData;
    }

    public void SetInventoryItem(List<InventorySaveData> inventorySaveData)
    {
        // Clear inventory panel - avoid duplicates
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        ItemDictionary itemDictionary = FindFirstObjectByType<ItemDictionary>();
        // Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                if (data.slotIndex < slotCount)
                {
                    InventorySlot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<InventorySlot>();
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

        RebuildItemCounts();
    }

    public void RemoveItemsFromInventory(ItemSO itemSO, int amountToRemove)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
            if (slot != null && slot.itemSO == itemSO && slot.quantity > 0)
            {
                int removed = Mathf.Min(amountToRemove, slot.quantity);
                slot.quantity -= removed;
                amountToRemove -= removed;

                if (slot.quantity <= 0)
                {
                    slot.itemSO = null;
                    slot.quantity = 0;
                }

                slot.UpdateUI();
            }
        }

        RebuildItemCounts();
    }

    public void RemoveItemsFromInventory(string itemName, int amountToRemove)
    {
        ItemDictionary itemDictionary = FindFirstObjectByType<ItemDictionary>();
        ItemSO itemSO = itemDictionary?.GetItemPrefab(itemName);
        if (itemSO != null)
        {
            RemoveItemsFromInventory(itemSO, amountToRemove);
        }
    }

    private void OnItemLooted(ItemSO itemSO, int quantity)
    {
        AddItem(itemSO, quantity);
    }
}
