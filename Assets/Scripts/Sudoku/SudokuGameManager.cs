using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGameManager : MonoBehaviour
{
    public static int gridLength = 9;
    public static int subGridLength = 3;

    public static int cellLength = SudokuGameManager.gridLength / SudokuGameManager.subGridLength;

    public SudokuCell SudokuCell_Prefab;

    public static SudokuGameManager Instance { get; private set; }

    public MainMenuPanel mainMenu;
    public SudokuPlayPanel sudokuPlay;

    public int difficulty = 20;

    public Image ipButtonsPanel;

    public List<Button> btnNums = new List<Button>();

    SudokuCell lastCell;

    public TimeBasedRewardConfig rewardConfig;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < btnNums.Count; i++)
        {
            int index = i;
            btnNums[i].onClick.AddListener(delegate () { OnNumBtnClicked(index); });
        }
    }

    private void Start()
    {
        OnBackToMenu();
    }

    public void OnBackToMenu()
    {
        mainMenu.gameObject.SetActive(true);
        sudokuPlay.gameObject.SetActive(false);

        sudokuPlay.Clear();
    }

    public void OnPlayNewGame()
    {
        mainMenu.gameObject.SetActive(false);
        sudokuPlay.gameObject.SetActive(true);

        ipButtonsPanel.gameObject.SetActive(false);

        sudokuPlay.Init();
    }

    public void ActivateInputButton(SudokuCell cell)
    {
        ipButtonsPanel.gameObject.SetActive(true);
        lastCell = cell;
    }

    public void OnNumBtnClicked(int num)
    {
        lastCell.UpdateValue(num);
        sudokuPlay.board.UpdatePuzzle(lastCell.coordinate.x, lastCell.coordinate.y, num);
        ipButtonsPanel.gameObject.SetActive(false);
    }
}
