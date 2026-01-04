using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public string itemName;
    public int slotIndex; // The index of the slot where the item is placed within our inventory
    public int quantity = 1;
}
