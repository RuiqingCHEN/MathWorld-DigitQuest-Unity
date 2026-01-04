using UnityEngine;

public class RewardsController : MonoBehaviour
{
    public static RewardsController Instance { get; private set; }
    public GameObject lootPrefab;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveQuestReward(Quest quest)
    {
        if (quest?.questRewards == null) return;

        foreach (var reward in quest.questRewards)
        {
            switch (reward.type)
            {
                case RewardType.Item:
                    GiveItemReward(reward.rewardID, reward.amount);
                    break;
                case RewardType.Gold:
                    GiveGoldReward(reward.amount);
                    break;
                case RewardType.Experience:
                    break;
                case RewardType.Custom:
                    break;

            }
        }
    }

    public void GiveItemReward(string itemName, int amount)
    {
        ItemDictionary itemDictionary = FindFirstObjectByType<ItemDictionary>();
        ItemSO itemSO = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemName);

        if (itemSO == null) return;

        bool itemAdded = InventoryController.Instance.AddItem(itemSO, amount);

        if (itemAdded)
        {
            if (ItemPickupUIController.Instance != null)
            {
                ItemPickupUIController.Instance.ShowItemPickup(itemSO.itemName, itemSO.icon);
            }
        }
        else
        {
            DropItemReward(itemSO, amount);
        }
    }

    private void DropItemReward(ItemSO itemSO, int amount)
    {
        if (lootPrefab == null)
        {
            return;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        Vector3 dropPosition = playerTransform != null ? playerTransform.position : transform.position;

        GameObject dropLoot = Instantiate(lootPrefab, dropPosition + Vector3.down, Quaternion.identity);
        Loot lootComponent = dropLoot.GetComponent<Loot>();

        if (lootComponent != null)
        {
            lootComponent.Initialize(itemSO, amount);
            BounceEffect bounceEffect = dropLoot.GetComponent<BounceEffect>();
            if (bounceEffect != null)
            {
                bounceEffect.StartBounce();
            }
        }
    }

    public void GiveGoldReward(int amount)
    {
        ItemSO itemSO = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab("GOLD COIN");

        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.gold += amount;
            InventoryController.Instance.goldText.text = InventoryController.Instance.gold.ToString();
        }

        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(itemSO.itemName, itemSO.icon);
        }
    }

    public void GiveScoreBasedReward(ScoreBasedRewardConfig config, int score)
    {
        if (config == null) return;

        int goldToGive = 0;

        foreach (var tier in config.scoreTiers)
        {
            if (score >= tier.scoreThreshold && tier.goldAmount > goldToGive)
                goldToGive = tier.goldAmount;
        }

        if (goldToGive > 0)
            GiveGoldReward(goldToGive);
    }

    public void GiveTimeBasedReward(TimeBasedRewardConfig config, bool completed, float timeTaken)
    {
        if (config == null || !completed) 
            return;

        int baseGold;
        float difficultyMultiplier;
        int difficulty = SudokuGameManager.Instance.difficulty;

        if (difficulty <= 35)
        {
            baseGold = 2;
            difficultyMultiplier = 1.0f;
        }
        else if (difficulty <= 50)
        {
            baseGold = 4;
            difficultyMultiplier = 1.5f;
        }
        else
        {
            baseGold = 6;
            difficultyMultiplier = 2.0f;
        }

        int bonusGold = 0;

        foreach (var tier in config.timeTiers)
        {
            float adjustedThreshold = tier.timeThreshold * difficultyMultiplier;

            if (timeTaken <= adjustedThreshold && tier.bonusGold > bonusGold)
            {
                bonusGold = tier.bonusGold;
            }
        }

        int totalGold = baseGold + bonusGold;
        GiveGoldReward(totalGold);
    }

}
