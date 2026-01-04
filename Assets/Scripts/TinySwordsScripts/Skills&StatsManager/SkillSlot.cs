using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class SkillSlot : MonoBehaviour
{
    public string slotID;
    
    public List<SkillSlot> prerequisiteSkillSlots;
    public SkillSO skillSO;
    public int currentLevel;
    public bool isUnlocked;
    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    // Action<SkillSlot> 它接收一个 SkillSlot 类型的参数，不返回值。
    public static event Action<SkillSlot> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    // runs anytime you make changes to a script's variable
    private void OnValidate()
    {
        if (skillSO != null && skillLevelText != null)
        {

            UpdateUI();
        }
    }

    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSO.maxLevel)
        {
            currentLevel++;
            OnAbilityPointSpent?.Invoke(this);

            if (currentLevel >= skillSO.maxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }
            UpdateUI();
        }
    }

    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked || slot.currentLevel < slot.skillSO.maxLevel)
            {
                return false;
            }
        }
        return true;
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (isUnlocked)
        {
            skillButton.interactable = true;
            skillLevelText.text = currentLevel.ToString() + " / " + skillSO.maxLevel.ToString();
            skillIcon.sprite = skillSO.skillIcon;
        }
        else
        {
            skillButton.interactable = false;
            skillLevelText.text = "Locked";
            skillIcon.sprite = skillSO.disabledSkillIcon;
        }
    }
}
