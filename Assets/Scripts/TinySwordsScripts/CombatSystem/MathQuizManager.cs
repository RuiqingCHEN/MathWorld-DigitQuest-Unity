using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MathQuizManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text number1Text;
    public TMP_Text operatorText;
    public TMP_Text number2Text;
    public TMP_Text resultText;
    public Button[] answerButtons;
    public Slider timerSlider;
    public TMP_Text correctCountText;
    public TMP_Text timerText;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 4;
    public float gameTime = 30f;

    private int correctAnswer;
    private int questionType;
    private int correctCount = 0;
    private float timeRemaining;
    private bool gameActive = true;

    [Header("Question Settings")]
    public QuestionConfig questionConfig;

    private int number1;
    private int number2;
    private int result;
    private OperationType currentOp;

    [Header("Battle Integration")]
    public BattleSystem battleSystem;

    void Start()
    {
        timeRemaining = gameTime;
        correctCount = 0;
        gameActive = false;

        // 初始化显示
        UpdateCorrectCountDisplay();
        GenerateNewQuestion();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].interactable = false;
        }
    }

    void Update()
    {
        if (gameActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame();
            }
        }
    }

    public void SetQuestionConfig(QuestionConfig config)
    {
        questionConfig = config;
    }

    void GenerateNewQuestion()
    {
        if (!gameActive) return;

        currentOp = questionConfig.operationType;
    
        switch (currentOp)
        {
            case OperationType.Addition:
                do {
                    number1 = Random.Range(questionConfig.minNumber, questionConfig.maxNumber + 1);
                    number2 = Random.Range(questionConfig.minNumber, questionConfig.maxNumber + 1);
                    result = number1 + number2;
                } while (!questionConfig.allowCarrying && HasCarrying(number1, number2));
                break;
                
            case OperationType.Subtraction:
                do {
                    number1 = Random.Range(questionConfig.minNumber, questionConfig.maxNumber + 1);
                    number2 = Random.Range(questionConfig.minNumber, number1 + 1); // 确保结果为正
                    result = number1 - number2;
                } while (!questionConfig.allowCarrying && HasBorrowing(number1, number2));
                break;
                
            case OperationType.Multiplication:
            case OperationType.Division:
                number2 = Random.Range(questionConfig.minNumber, questionConfig.maxNumber + 1);
                result = Random.Range(questionConfig.minNumber, questionConfig.maxNumber + 1);
                number1 = number2 * result;
                if (currentOp == OperationType.Multiplication)
                    (number1, result) = (result, number1);
                break;
        }

        questionType = Random.Range(0, 3);

        UpdateQuestionDisplay(questionType);
        GenerateAnswerOptions();
    }

    bool HasCarrying(int a, int b)
    {
        while (a > 0 || b > 0)
        {
            if ((a % 10) + (b % 10) >= 10) return true;
            a /= 10;
            b /= 10;
        }
        return false;
    }

    bool HasBorrowing(int a, int b)
    {
        while (a > 0 || b > 0)
        {
            if ((a % 10) < (b % 10)) return true;
            a /= 10;
            b /= 10;
        }
        return false;
    }

    void UpdateQuestionDisplay(int questionType)
    {
        // 先隐藏所有Image
        SetImageActive(number1Text, false);
        SetImageActive(number2Text, false);
        SetImageActive(resultText, false);

        operatorText.text = currentOp == OperationType.Addition ? "+" : 
                currentOp == OperationType.Subtraction ? "−" :
                currentOp == OperationType.Multiplication ? "×" : "÷";
        

        switch (questionType)
        {
            case 0: // number1是问号
                number1Text.text = "?";
                number2Text.text = number2.ToString();
                resultText.text = result.ToString();
                correctAnswer = number1;
                SetImageActive(number1Text, true);
                break;

            case 1: // number2是问号
                number1Text.text = number1.ToString();
                number2Text.text = "?";
                resultText.text = result.ToString();
                correctAnswer = number2;
                SetImageActive(number2Text, true);
                break;

            case 2: // result是问号
                number1Text.text = number1.ToString();
                number2Text.text = number2.ToString();
                resultText.text = "?";
                correctAnswer = result;
                SetImageActive(resultText, true);
                break;
        }
    }

    void SetImageActive(TMP_Text textComponent, bool isActive)
    {
        Image childImage = textComponent.GetComponentInChildren<Image>(true);
        if (childImage != null)
        {
            childImage.gameObject.SetActive(isActive);
        }
    }

    void GenerateAnswerOptions()
    {
        List<int> options = new List<int>();
        options.Add(correctAnswer);

        // 生成错误答案
        while (options.Count < 4)
        {
            int wrongAnswer;
            int variation = Random.Range(-3, 4);
            if (variation == 0) variation = 1;

            wrongAnswer = correctAnswer + variation;

            if (wrongAnswer != correctAnswer && !options.Contains(wrongAnswer) && wrongAnswer > 0)
            {
                options.Add(wrongAnswer);
            }
        }

        for (int i = 0; i < options.Count; i++)
        {
            int randomIndex = Random.Range(0, options.Count);
            int temp = options[i];
            options[i] = options[randomIndex];
            options[randomIndex] = temp;
        }

        for (int i = 0; i < answerButtons.Length && i < options.Count; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = options[i].ToString();
        }
    }

    public void OnAnswerButtonClicked(int buttonIndex)
    {
        if (!gameActive) return;

        string buttonText = answerButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text;
        int selectedAnswer = int.Parse(buttonText);

        if (selectedAnswer == correctAnswer)
        {
            correctCount++;
            Debug.Log($"答对了! 当前正确数: {correctCount}");
            UpdateCorrectCountDisplay();
        }
        else
        {
            Debug.Log("答错了!");
        }

        // 无论对错都生成新题目
        GenerateNewQuestion();
    }

    void UpdateTimerDisplay()
    {
        if (timerSlider != null)
        {
            timerSlider.value = timeRemaining;
            timerSlider.maxValue = gameTime;
        }

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = seconds + "s";
        }
    }

    // 更新答对题数显示
    void UpdateCorrectCountDisplay()
    {
        if (correctCountText != null)
        {
            correctCountText.text = correctCount.ToString();
        }
    }

    void EndGame()
    {
        gameActive = false;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].interactable = false;
        }

        ResetNumbers();

        if (battleSystem != null)
        {
            battleSystem.OnQuizComplete(correctCount);
        }
    }

    public void StartQuiz()
    {
        correctCount = 0;
        timeRemaining = gameTime;
        gameActive = true;
        UpdateCorrectCountDisplay();
        GenerateNewQuestion();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].interactable = true;
        }
    }

    void ResetNumbers()
    {
        number1Text.text = "?";
        operatorText.text = "+";
        number2Text.text = "?";
        resultText.text = "?";

        SetImageActive(number1Text, false);
        SetImageActive(number2Text, false);
        SetImageActive(resultText, false);
    }
}