using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }

    [Header("Loot Settings")]
    public GameObject lootPrefab; 
    public ItemSO itemToDrop; // Item that chest drops
    public int dropQuantity = 1; 

    public Sprite openedSprite;
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);// UniqueID
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }
    public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();

    }

    public void OpenChest()
    {
        SetOpened(true);
        SoundEffectManager.Play("Chest");
        if (lootPrefab != null && itemToDrop != null)
        {
            GameObject droppedItem = Instantiate(lootPrefab, transform.position + Vector3.down * 0.7f, Quaternion.identity);

            droppedItem.GetComponent<Loot>().Initialize(itemToDrop, dropQuantity); 

            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }

    }

    public void SetOpened(bool opened)
    {
        if (IsOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
