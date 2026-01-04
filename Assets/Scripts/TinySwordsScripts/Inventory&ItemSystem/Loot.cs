using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int quantity = 1;
    public static event Action<ItemSO, int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        UpdateAppearance();
    }

    public void Initialize(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        spriteRenderer.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && itemSO != null)
        {
            animator.Play("LootPickup");
            OnItemLooted?.Invoke(itemSO, quantity);
            ShowPopUp();
            Destroy(gameObject, .5f);
        }
    }

    public virtual void ShowPopUp()
    {
        if(ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(itemSO.itemName, itemSO.icon);
        }
    }
}
