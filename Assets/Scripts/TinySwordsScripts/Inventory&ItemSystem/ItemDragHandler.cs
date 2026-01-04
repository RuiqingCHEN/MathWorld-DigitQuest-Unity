using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Ghost Prefab Settings")]
    public GameObject ghostPrefab; 
    
    [Header("Drop Settings")]
    public float minDropDistance = 1.2f;
    public float maxDropDistance = 1.5f;
    
    private InventorySlot originalSlot;
    private InventoryController inventoryController;
    private GameObject ghostInstance; 
    private Canvas canvas; 

    void Start()
    {
        inventoryController = InventoryController.Instance;
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = GetComponentInParent<InventorySlot>();
        if (originalSlot == null || originalSlot.itemSO == null) return;

        CreateGhostPrefab();
        
        if (originalSlot.itemImage != null)
        {
            originalSlot.itemImage.color = new Color(1f, 1f, 1f, 0.3f); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostInstance != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint);
            
            ghostInstance.transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (originalSlot != null && originalSlot.itemImage != null)
        {
            originalSlot.itemImage.color = Color.white;
        }

        DestroyGhost();

        if (originalSlot == null || originalSlot.itemSO == null) return;

        InventorySlot dropSlot = GetDropSlot(eventData);
        
        if (dropSlot == null)
        {
            if (!IsWithinInventory(eventData.position))
            {
                DropItemToWorld();
            }
            return;
        }

        if (dropSlot == originalSlot) return;

        HandleDrop(dropSlot);
    }

    private void CreateGhostPrefab()
    {
        if (ghostPrefab == null || originalSlot == null) return;

        ghostInstance = Instantiate(ghostPrefab, canvas.transform);

        Image ghostImage = ghostInstance.GetComponent<Image>();
        TMP_Text ghostText = ghostInstance.GetComponentInChildren<TMP_Text>();
        if (ghostImage != null && originalSlot.itemSO != null && ghostText != null)
        {
            ghostImage.sprite = originalSlot.itemSO.icon;
            ghostImage.color = new Color(1f, 1f, 1f, 0.8f);
            ghostText.text = originalSlot.quantity.ToString();
        }

        ghostInstance.transform.position = originalSlot.transform.position;

        ghostInstance.transform.SetAsLastSibling();
        
        CanvasGroup ghostCanvasGroup = ghostInstance.GetComponent<CanvasGroup>();
        if (ghostCanvasGroup == null)
        {
            ghostCanvasGroup = ghostInstance.AddComponent<CanvasGroup>();
        }
        ghostCanvasGroup.blocksRaycasts = false;
    }

    private void DestroyGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }
    }

    private InventorySlot GetDropSlot(PointerEventData eventData)
    {
        InventorySlot dropSlot = eventData.pointerEnter?.GetComponent<InventorySlot>();
        
        if (dropSlot == null)
        {
            GameObject dropObject = eventData.pointerEnter;
            if (dropObject != null)
            {
                dropSlot = dropObject.GetComponentInParent<InventorySlot>();
            }
        }
        
        return dropSlot;
    }

    private void HandleDrop(InventorySlot dropSlot)
    {
        if (dropSlot.itemSO != null)
        {
            if (dropSlot.itemSO == originalSlot.itemSO)
            {
                TryStackItems(dropSlot);
            }
            else
            {
                SwapItems(dropSlot);
            }
        }
        else
        {
            MoveItemToSlot(dropSlot);
        }
    }

    private void TryStackItems(InventorySlot targetSlot)
    {
        ItemSO itemSO = originalSlot.itemSO;
        int availableSpace = itemSO.stackSize - targetSlot.quantity;
        int amountToStack = Mathf.Min(availableSpace, originalSlot.quantity);

        if (amountToStack > 0)
        {
            targetSlot.quantity += amountToStack;
            targetSlot.UpdateUI();

            originalSlot.quantity -= amountToStack;
            
            if (originalSlot.quantity <= 0)
            {
                originalSlot.itemSO = null;
                originalSlot.quantity = 0;
            }
            
            originalSlot.UpdateUI();
            
            inventoryController.RebuildItemCounts();
        }
    }

    private void SwapItems(InventorySlot targetSlot)
    {
        ItemSO targetItemSO = targetSlot.itemSO;
        int targetQuantity = targetSlot.quantity;

        targetSlot.itemSO = originalSlot.itemSO;
        targetSlot.quantity = originalSlot.quantity;
        targetSlot.UpdateUI();

        originalSlot.itemSO = targetItemSO;
        originalSlot.quantity = targetQuantity;
        originalSlot.UpdateUI();

        inventoryController.RebuildItemCounts();
    }

    private void MoveItemToSlot(InventorySlot targetSlot)
    {
        targetSlot.itemSO = originalSlot.itemSO;
        targetSlot.quantity = originalSlot.quantity;
        targetSlot.UpdateUI();

        originalSlot.itemSO = null;
        originalSlot.quantity = 0;
        originalSlot.UpdateUI();

        inventoryController.RebuildItemCounts();
    }

    private bool IsWithinInventory(Vector2 mousePosition)
    {
        HotbarController hotbarController = FindFirstObjectByType<HotbarController>();
        bool isInHotbar = false;
        if (hotbarController != null)
        {
            RectTransform hotbarRect = hotbarController.hotbarPanel.GetComponent<RectTransform>();
            isInHotbar = RectTransformUtility.RectangleContainsScreenPoint(hotbarRect, mousePosition);
        }
        
        MenuController menuController = FindFirstObjectByType<MenuController>();
        bool isInInventory = false;
        if (menuController != null && menuController.menuCanvas.activeSelf)
        {
            RectTransform inventoryRect = inventoryController.inventoryPanel.GetComponent<RectTransform>();
            isInInventory = RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
        }
        
        return isInHotbar || isInInventory;
    }

    private void DropItemToWorld()
    {
        ItemSO itemToDrop = originalSlot.itemSO;
        int quantityToDrop = originalSlot.quantity;

        originalSlot.itemSO = null;
        originalSlot.quantity = 0;
        originalSlot.UpdateUI();

        SpawnDroppedItem(itemToDrop, quantityToDrop);
        
        inventoryController.RebuildItemCounts();
    }

    private void SpawnDroppedItem(ItemSO itemSO, int quantity)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }

        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        GameObject dropItem = Instantiate(inventoryController.lootPrefab, dropPosition, Quaternion.identity);
        Loot loot = dropItem.GetComponent<Loot>();
        if (loot != null)
        {
            loot.Initialize(itemSO, quantity);
            
            BounceEffect bounceEffect = dropItem.GetComponent<BounceEffect>();
            if (bounceEffect != null)
            {
                bounceEffect.StartBounce();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }

    private void SplitStack()
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        if (slot == null || slot.itemSO == null || slot.quantity <= 1) return;

        int splitAmount = slot.quantity / 2;
        if (splitAmount <= 0) return;

        slot.quantity -= splitAmount;
        slot.UpdateUI();

        InventorySlot emptySlot = inventoryController.FindEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.itemSO = slot.itemSO;
            emptySlot.quantity = splitAmount;
            emptySlot.UpdateUI();
        }
        else
        {
            slot.quantity += splitAmount;
            slot.UpdateUI();
        }

        inventoryController.RebuildItemCounts();
    }
}


