using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum QuestState { NotStarted, InProgress, Completed }
    private QuestState questState = QuestState.NotStarted;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        // 如果游戏暂停且没有对话正在进行，或者根本没有对话内容，就不执行交互逻辑。
        // If no dialogue data or the game is paused and no dialogue is active
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive && !(dialogueData.canBattle && BattleManager.GetCurrentBattleConfig() != null)))
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        // Sync同步 with quest data
        SyncQuestState();

        if (dialogueData.canBattle && BattleManager.GetCurrentBattleConfig() != null)
        {
            bool battleWon = BattleManager.GetBattleResult();
            if (battleWon)
            {
                dialogueIndex = dialogueData.battleWonDialogueIndex;
            }
            else if (!battleWon)
            {
                dialogueIndex = dialogueData.battleLostDialogueIndex;
            }
        }
        else
        {
            // Set dialogue line based on questState
            if (questState == QuestState.NotStarted)
            {
                dialogueIndex = 0;
            }
            else if (questState == QuestState.InProgress)
            {
                dialogueIndex = dialogueData.questInProgressIndex;
            }
            else if (questState == QuestState.Completed)
            {
                dialogueIndex = dialogueData.questCompletedIndex;
            }
        }
        
        isDialogueActive = true;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;
        string questID = dialogueData.quest.questID;

        // Future update add completing quest and handling in!
        if (QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }

    }

    /*
        第0行设置了自动播放 
        Line0(Auto)->Interact->TypeLine->NextLine

        第1行设置了choiceButton且不自动播放
        NextLine->dialogueChoice.dialogueIndex=1 / dialogueIndex=0 ->++dialogueIndex->TypeLine
        显示第1行内容 因无自动播放 不会调用NextLine 接着按E
        Interact->NextLine->dialogueChoice.dialogueIndex=dialogueIndex=1->displaychoices->return
        显示按钮 不显示下一行

        第1行设置了choiceButton且自动播放
        NextLine->dialogueChoice.dialogueIndex=1 / dialogueIndex=0 ->++dialogueIndex->TypeLine
        ->NextButton->后和按E效果相同 都是调用NextLine
    */
    void NextLine()
    {
        if (isTyping)
        {
            // Skip typing animation and show the full line
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        // Clear Choices
        dialogueUI.ClearChoices();
        // Check endDialogueLines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }
        // Check if choices & display
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // If another line, type next line
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // 判断当前这行对话是否应该自动播放，如果是，就等待一段时间再继续。
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            bool startsBattle = choice.startsBattle[i];
            bool opensShop = choice.opensShop[i];
            bool opensMiniGame = choice.opensMiniGame[i];

            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest, startsBattle, opensShop, opensMiniGame));
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest, bool startsBattle, bool opensShop, bool opensMiniGame)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }

        if (startsBattle)
        {
            StartBattle();
            return;
        }

        if (opensShop)
        {
            OpenShop();
            return;
        }

        if (opensShop)
        {
            OpenShop();
            return;
        }

        if (opensMiniGame)
        {
            OpenMiniGame();
            return;
        }

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void OpenShop()
    {
        dialogueUI.ClearChoices();
        EndDialogue(); 
        ShopKeeper shopKeeper = GetComponent<ShopKeeper>();
        if (shopKeeper != null)
        {
            shopKeeper.OpenShop();
        }
    }

    void OpenMiniGame()
    {
        dialogueUI.ClearChoices();
        EndDialogue(); 
        MiniGameKeeper miniGameKeeper = GetComponent<MiniGameKeeper>();
        if (miniGameKeeper != null)
        {
            miniGameKeeper.OpenMiniGame();
        }
    }

    void StartBattle()
    {
        dialogueUI.ClearChoices();

        EndDialogue();

        if (dialogueData.canBattle && dialogueData.battleConfig != null)
        {
            BattleManager.Instance.StartBattle(dialogueData.battleConfig, this);
        }
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        if (dialogueData.canBattle && BattleManager.GetCurrentBattleConfig() != null)
        {
            if (BattleManager.GetBattleResult())
            {
                GiveBattleRewards();
            }
            BattleManager.ClearBattleResult(); 
        }
        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            HandleQuestCompletion(dialogueData.quest);
        }

        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    void HandleQuestCompletion(Quest quest)
    {
        RewardsController.Instance.GiveQuestReward(quest);
        QuestController.Instance.HandInQuest(quest.questID);
    }
    
    private void GiveBattleRewards()
    {
        if (dialogueData.battleConfig?.enemyData?.battleRewards == null) return;

        foreach (var reward in dialogueData.battleConfig.enemyData.battleRewards)
        {
            RewardsController.Instance.GiveItemReward(reward.rewardID, reward.amount);
        }
    }
}
