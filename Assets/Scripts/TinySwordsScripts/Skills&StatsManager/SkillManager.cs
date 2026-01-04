using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }
    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }
    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        string skillName = slot.skillSO.skillName;
        switch (skillName)
        {
            case "Heal":
                PlayerData.Instance.UpdateMaxHealth(1);
                break;
            case "Attack Upgrade":
                PlayerData.Instance.UpdateDamage(1);
                break;
            case "Defense Upgrade":
                PlayerData.Instance.UpdateDefense(1);
                break;
            case "Speed":
                PlayerData.Instance.UpdateSpeed(1);
                break;
            default:
                Debug.LogWarning("Unknown skill: " + skillName);
                break;
        }
    }
}
