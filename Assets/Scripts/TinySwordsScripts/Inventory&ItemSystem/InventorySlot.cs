using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public ItemSO itemSO;
    public int quantity;

    public Image itemImage;
    public TMP_Text quantityText;

    private static ShopManager activeShop;
    private ShopInfo shopInfo;

    private void Start()
    {
        if (shopInfo == null)
        {
            GameObject shopInfoObj = GameObject.Find("InventorySlotInfo");
            if (shopInfoObj != null)
            {
                shopInfo = shopInfoObj.GetComponent<ShopInfo>();
            }
        }

        UpdateUI();
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopStateChanged += HandleShopStateChanged;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopStateChanged -= HandleShopStateChanged;
    }

    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (activeShop != null)
                {
                    activeShop.SellItem(itemSO);
                    quantity--;
                    UpdateUI();
                }
                else
                {
                    if (itemSO.currentHP > 0 && PlayerData.Instance.currentHP >= PlayerData.Instance.maxHP)
                        return;
                }
            }
        }
    }

    public void UpdateUI()
    {
        if (quantity <= 0)
            itemSO = null;

        if (itemSO != null)
        {
            itemImage.sprite = itemSO.icon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSO != null && shopInfo != null && IsMenuOpen())
            shopInfo.ShowItemInfo(itemSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopInfo != null && IsMenuOpen())
            shopInfo.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSO != null && shopInfo != null && IsMenuOpen())
            shopInfo.FollowMouse();
    }
    
    private bool IsMenuOpen()
    {
        MenuController menuController = FindFirstObjectByType<MenuController>();
        return menuController != null && menuController.menuCanvas.activeInHierarchy;
    }
}
