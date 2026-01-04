using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public Button btnPlay;
    public Slider sldDifficulty;

    private void Awake()
    {
        sldDifficulty.onValueChanged.AddListener(OnSliderChange);

        btnPlay.onClick.AddListener(OnPlayGame);
    }

    void Start()
    {
        SudokuGameManager.Instance.difficulty = (int)sldDifficulty.value;
    }

    public void OnSliderChange(float value)
    {
        SudokuGameManager.Instance.difficulty = (int)sldDifficulty.value;
    }

    public void OnPlayGame()
    {
        SudokuGameManager.Instance.OnPlayNewGame();
    }
}
