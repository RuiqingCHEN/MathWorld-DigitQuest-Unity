using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public enum BattleState { START, PLAYERTURN, QUIZTURN, ENEMYTURN, WON ,LOST }
public class BattleSystem : MonoBehaviour
{
    public Image playerPrefab;
    public Image enemyPrefab;

    public Image playerBattleStation;
    public Image enemyBattleStation;

    public TMP_Text dialogueText;
    public TMP_Text turnText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    Unit playerUnit;
    Unit enemyUnit;

    public BattleState state;

    public delegate void BattleVictory(int exp);
    public static event BattleVictory OnBattleWon;

    [Header("Quiz Integration")]
    public MathQuizManager mathQuizManager;


    void OnEnable()
    {
        if (BattleManager.GetCurrentBattleConfig() != null)
        {
            state = BattleState.START;
            StartCoroutine(SetupBattle());
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (playerUnit != null)
            Destroy(playerUnit.gameObject);
        if (enemyUnit != null)
            Destroy(enemyUnit.gameObject);
    }

    IEnumerator SetupBattle()
    {
        Image playerGO = Instantiate(playerPrefab, playerBattleStation.transform);
        playerUnit = playerGO.GetComponent<Unit>();
        playerUnit.maxHP = PlayerData.Instance.maxHP;
        playerUnit.currentHP = PlayerData.Instance.currentHP;

        Image enemyGO = Instantiate(enemyPrefab, enemyBattleStation.transform);
        enemyUnit = enemyGO.GetComponent<Unit>();

        NPCBattleConfig battleConfig = BattleManager.GetCurrentBattleConfig();
        if (battleConfig != null)
        {
            EnemyData enemyData = battleConfig.enemyData;
            enemyUnit.Initialize(enemyData.enemyName, enemyData.level, enemyData.maxHP, enemyData.damage, enemyData.aliveSprite, enemyData.deadSprite);

            if (mathQuizManager != null && enemyData.questionConfig != null)
            {
                mathQuizManager.SetQuestionConfig(enemyData.questionConfig);
            }
        
        }

        dialogueText.text = "A wild " + enemyUnit.unitName + "approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);

        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        turnText.text = "Enemy Turn";
        turnText.color = Color.red;
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage - PlayerData.Instance.defense);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        turnText.text = "";
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
            StartCoroutine(HandleBattle(true));
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
            StartCoroutine(HandleBattle(false));
        }
    }

    IEnumerator HandleBattle(bool isWin)
    {
        PlayerData.Instance.currentHP = playerUnit.currentHP;

        if (isWin)
        {
            NPCBattleConfig battleConfig = BattleManager.GetCurrentBattleConfig();
            if (battleConfig != null && battleConfig.enemyData != null)
                OnBattleWon?.Invoke(battleConfig.enemyData.expReward);
        }

        yield return new WaitForSeconds(3f);
        BattleManager.Instance.EndBattle(isWin);
    }

    void PlayerTurn()
    {
        turnText.text = "Your Turn";
        turnText.color = Color.blue;
        dialogueText.text = "Get ready to answer questions!";
        StartCoroutine(EnterQuizTurn());
    }

    IEnumerator EnterQuizTurn()
    {
        yield return new WaitForSeconds(2f);
        
        state = BattleState.QUIZTURN;
        StartQuizTurn();
    }

    void StartQuizTurn()
    {
        turnText.text = "Quiz Turn";
        dialogueText.text = "Answer math questions to increase your attack power!";
        
        if (mathQuizManager != null)
        {
            mathQuizManager.StartQuiz();
        }
    }

    public void OnQuizComplete(int correctAnswers)
    {
        playerUnit.damage = correctAnswers + PlayerData.Instance.damage;
        dialogueText.text = $"Quiz complete! You answered {correctAnswers} questions correctly. Attack power: {PlayerData.Instance.damage}+ {playerUnit.damage}";
        
        state = BattleState.PLAYERTURN;
        StartCoroutine(ShowPlayerChoice());
    }

    IEnumerator ShowPlayerChoice()
    {
        yield return new WaitForSeconds(2f);
        dialogueText.text = "Choose an action: ";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        
        StartCoroutine(PlayerAttack());
    }
    
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());    
    }
}
