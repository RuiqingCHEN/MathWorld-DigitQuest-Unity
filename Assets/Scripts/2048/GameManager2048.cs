using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager2048 : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public TMP_Text txtScore;
    public TMP_Text txtBestScore;

    private int score = 0;
    private int bestScore = 0;

    public ScoreBasedRewardConfig rewardConfig;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        txtBestScore.text = bestScore.ToString();

        SetScore(0);

        gameOver.alpha = 0;
        gameOver.interactable = false;

        board.enabled = true;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();

    }

    void SetScore(int score)
    {
        this.score = score;
        txtScore.text = score.ToString();

        if (this.score > bestScore)
        {
            bestScore = this.score;
            txtBestScore.text = bestScore.ToString();
            PlayerPrefs.SetInt("bestScore", this.score);
        }
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        if (rewardConfig != null)
            RewardsController.Instance.GiveScoreBasedReward(rewardConfig, score);

        PlayerPrefs.Save();

        StartCoroutine(FadeAnimate(gameOver, 1.0f));
    }

    IEnumerator FadeAnimate(CanvasGroup canvasGroup, float fadeTo, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;

        float fadeFrom = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = fadeTo;
    }
}
