using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonToggles : MonoBehaviour
{
    public Transform buttonParent;
    public GameObject buttonPrefab;

    private void Start()
    {
        GenerateCategoryButtons();
    }
    void GenerateCategoryButtons()
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        if (ShopKeeper.currentShopKeeper == null)
            return;

        var categories = ShopKeeper.currentShopKeeper.GetShopCategories();

        for (int i = 0; i < categories.Count; i++)
        {
            int categoryIndex = i;
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = categories[i].categoryName;
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OpenShopCategory(categoryIndex));
            }
        }
    }

    public void OpenShopCategory(int categoryIndex)
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenCategoryShop(categoryIndex);
        }
    }
    
    public void OnShopOpened()
    {
        GenerateCategoryButtons();
    }
}
