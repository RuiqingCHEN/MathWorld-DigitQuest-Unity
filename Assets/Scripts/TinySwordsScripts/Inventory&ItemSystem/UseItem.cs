using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffects(ItemSO itemSO)
    {
        if (itemSO.currentHP > 0)
            PlayerData.Instance.UpdateHealth(itemSO.currentHP);

        if (itemSO.maxHP > 0)
            PlayerData.Instance.UpdateMaxHealth(itemSO.maxHP);

        if (itemSO.speed > 0)
            PlayerData.Instance.UpdateSpeed(itemSO.speed);

        if (itemSO.damage > 0)
            PlayerData.Instance.UpdateDamage(itemSO.damage);

        if (itemSO.defense > 0)
            PlayerData.Instance.UpdateDefense(itemSO.defense);

        if (itemSO.duration > 0)
            StartCoroutine(EffectTimer(itemSO, itemSO.duration));
    }

    private IEnumerator EffectTimer(ItemSO itemSO, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (itemSO.currentHP > 0)
            PlayerData.Instance.UpdateHealth(-itemSO.currentHP);

        if (itemSO.maxHP > 0)
            PlayerData.Instance.UpdateMaxHealth(-itemSO.maxHP);

        if (itemSO.speed > 0)
            PlayerData.Instance.UpdateSpeed(-itemSO.speed);

        if (itemSO.damage > 0)
            PlayerData.Instance.UpdateDamage(-itemSO.damage);
            
        if (itemSO.defense > 0)
            PlayerData.Instance.UpdateDefense(-itemSO.defense);
    }
}
