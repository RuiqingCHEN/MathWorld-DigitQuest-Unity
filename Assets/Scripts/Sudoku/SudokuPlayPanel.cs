using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SudokuPlayPanel : MonoBehaviour
{
    public Button btnBackToMenu;
    public Button btnBackToMenuInWinPanel;
    public Button btnNewLevel;
    public Button btnReplay;
    public Button btnComplete;
    public TMP_Text txtWrongTips;
    public Image imgWinPanel;
    public TMP_Text txtTimer;
    float levelStartTime = 0f;

    public SudokuBoard board;

    private void Awake()
    {
        btnBackToMenu.onClick.AddListener(OnBtnBackToMenuClicked);
        btnBackToMenuInWinPanel.onClick.AddListener(OnBtnBackToMenuClicked);

        btnNewLevel.onClick.AddListener(OnBtnNewClicked);

        btnReplay.onClick.AddListener(OnBtnReplayClicked);

        btnComplete.onClick.AddListener(OnBtnCompleteClicked);
    }

    void Update()
    {
        if (levelStartTime > 0)
        {
            CountTimer();
        }
    }

    public void OnBtnBackToMenuClicked()
    {
        Clear();

        SudokuGameManager.Instance.OnBackToMenu();
    }

    public void OnBtnNewClicked()
    {
        Clear();
        Init();
    }

    public void OnBtnReplayClicked()
    {
        board.RestartGame();
    }

    public void OnBtnCompleteClicked()
    {
        if (board.CheckComplete())
        {
            imgWinPanel.gameObject.SetActive(true);

            float timeTaken = Time.realtimeSinceStartup - levelStartTime;
            if (SudokuGameManager.Instance.rewardConfig != null && RewardsController.Instance != null)
            {
                RewardsController.Instance.GiveTimeBasedReward(
                    SudokuGameManager.Instance.rewardConfig, 
                    completed: true, 
                    timeTaken
                );
            }
        }
        else
        {
            txtWrongTips.gameObject.SetActive(true);
            StartCoroutine(HideWrongText());
        }
    }

    IEnumerator HideWrongText()
    {
        yield return new WaitForSeconds(3.0f);
        txtWrongTips.gameObject.SetActive(false);
    }

    public void Init()
    {
        txtWrongTips.gameObject.SetActive(false);

        imgWinPanel.gameObject.SetActive(false);

        levelStartTime = Time.realtimeSinceStartup;

        board.Init();
    }

    public void Clear()
    {
        levelStartTime = 0;

        board.Clear();
    }

    void CountTimer()
    {
        float t = Time.realtimeSinceStartup - levelStartTime;
        int seconds = (int)(t % 60);
        t /= 60;
        int minutes = (int)(t % 60);

        txtTimer.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
    }
}
