using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopCategory> shopCategories;

    [SerializeField] private Camera shopKeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -1);

    public static event Action<ShopManager, bool> OnShopStateChanged;

    public List<ShopCategory> GetShopCategories()
    {
        return shopCategories;
    }

    public void OpenShop()
    {
        PauseController.SetPause(true);
        currentShopKeeper = this;
        OnShopStateChanged?.Invoke(shopManager, true);
        shopCanvasGroup.alpha = 1;
        shopCanvasGroup.blocksRaycasts = true;
        shopCanvasGroup.interactable = true;

        shopKeeperCam.transform.position = transform.position + cameraOffset;
        shopKeeperCam.gameObject.SetActive(true);

        ShopButtonToggles toggles = FindFirstObjectByType<ShopButtonToggles>();
        if (toggles != null)
        {
            toggles.OnShopOpened();
        }

        OpenCategoryShop(0);
    }

    public void CloseShop()
    {
        PauseController.SetPause(false);
        currentShopKeeper = null;
        OnShopStateChanged?.Invoke(shopManager, false);
        shopCanvasGroup.alpha = 0;
        shopCanvasGroup.blocksRaycasts = false;
        shopCanvasGroup.interactable = false;

        shopKeeperCam.gameObject.SetActive(false);
    }

    public void OpenCategoryShop(int categoryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < shopCategories.Count)
        {
            shopManager.PopulateShopItems(shopCategories[categoryIndex].items);
        }
    }
}

[System.Serializable]
public class ShopCategory
{
    public string categoryName; 
    public List<ShopItems> items; 
}
