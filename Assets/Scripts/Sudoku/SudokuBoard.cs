using System.Collections.Generic;
using UnityEngine;

public class SudokuBoard : MonoBehaviour
{
    public int[,] gridNumber = new int[SudokuGameManager.gridLength, SudokuGameManager.gridLength];
    public int[,] puzzleNumber = new int[SudokuGameManager.gridLength, SudokuGameManager.gridLength];
    public int[,] puzzleBak = new int[SudokuGameManager.gridLength, SudokuGameManager.gridLength];
    public SudokuGrid grid;

    public void Init()
    {
        CreateGrid();

        CreatePuzzle();

        InitButtons();
    }

    void CreateGrid()
    {
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> colList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int value = rowList[Random.Range(0, rowList.Count)];
        gridNumber[0, 0] = value;
        rowList.Remove(value);
        colList.Remove(value);

        for (int i = 1; i < SudokuGameManager.gridLength; i++)
        {
            value = rowList[Random.Range(0, rowList.Count)];
            gridNumber[i, 0] = value;
            rowList.Remove(value);
        }

        for (int i = 1; i < SudokuGameManager.gridLength; i++)
        {
            value = colList[Random.Range(0, colList.Count)];
            if (i < 3)
            {
                while (SquareContainsValue(0, 0, value))
                {
                    value = colList[Random.Range(0, colList.Count)];
                }
            }

            gridNumber[0, i] = value;
            colList.Remove(value);
        }

        for (int i = 6; i < 9; i++)
        {
            value = Random.Range(1, 10);
            while (SquareContainsValue(0, 8, value) || SquareContainsValue(8, 0, value) || SquareContainsValue(8, 8, value))
            {
                value = Random.Range(1, 10);
            }

            gridNumber[i, i] = value;
        }

        SolveSudoku();
    }

    bool SolveSudoku()
    {
        int row = 0;
        int col = 0;

        if (IsValid())
        {
            return true;
        }

        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                if (gridNumber[i, j] == 0)
                {
                    row = i;
                    col = j;
                    break;
                }
            }
        }

        for (int i = 1; i <= SudokuGameManager.gridLength; i++)
        {
            if (CheckAll(row, col, i))
            {
                gridNumber[row, col] = i;

                if (SolveSudoku())
                {
                    return true;
                }
                else
                {
                    gridNumber[row, col] = 0;
                }
            }
        }

        return false;
    }

    bool IsValid()
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                if (gridNumber[i, j] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool CheckAll(int row, int col, int value)
    {
        if (ColumnContainsValue(col, value))
        {
            return false;
        }

        if (RowContainsValue(row, value))
        {
            return false;
        }

        if (SquareContainsValue(row, col, value))
        {
            return false;
        }

        return true;
    }

    bool ColumnContainsValue(int col, int value)
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            if (gridNumber[i, col] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool RowContainsValue(int row, int value)
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            if (gridNumber[row, i] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool SquareContainsValue(int row, int col, int value)
    {
        for (int i = 0; i < SudokuGameManager.subGridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.subGridLength; j++)
            {
                if (gridNumber[(row / SudokuGameManager.subGridLength) * SudokuGameManager.cellLength + i, (col / SudokuGameManager.subGridLength) * SudokuGameManager.cellLength + j] == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CreatePuzzle()
    {
        System.Array.Copy(gridNumber, puzzleNumber, gridNumber.Length);

        for (int i = 0; i < SudokuGameManager.Instance.difficulty; i++)
        {
            int row = Random.Range(0, SudokuGameManager.gridLength);
            int col = Random.Range(0, SudokuGameManager.gridLength);

            while (puzzleNumber[row, col] == 0)
            {
                row = Random.Range(0, SudokuGameManager.gridLength);
                col = Random.Range(0, SudokuGameManager.gridLength);
            }

            puzzleNumber[row, col] = 0;
        }

        List<int> onBoard = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        RandomizeList(onBoard);

        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                for (int k = 0; k < onBoard.Count - 1; k++)
                {
                    if (onBoard[k] == puzzleNumber[i, j])
                    {
                        onBoard.RemoveAt(k);
                    }
                }
            }
        }

        while (onBoard.Count - 1 > 1)
        {
            int row = Random.Range(0, SudokuGameManager.gridLength);
            int col = Random.Range(0, SudokuGameManager.gridLength);

            if (gridNumber[row, col] == onBoard[0])
            {
                puzzleNumber[row, col] = gridNumber[row, col];
                onBoard.RemoveAt(0);
            }
        }

        System.Array.Copy(puzzleNumber, puzzleBak, gridNumber.Length);
    }

    void RandomizeList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void InitButtons()
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                var cell = grid.GetCellByPosition(i, j);
                if (cell != null)
                {
                    cell.InitValues(puzzleNumber[i, j]);
                }
            }
        }
    }

    public void UpdatePuzzle(int row, int col, int value)
    {
        puzzleNumber[row, col] = value;
    }

    public bool CheckComplete()
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                if (puzzleNumber[i, j] != gridNumber[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public void RestartGame()
    {
        System.Array.Copy(puzzleBak, puzzleNumber, puzzleBak.Length);
        InitButtons();
    }

    public void Clear()
    {
        for (int i = 0; i < SudokuGameManager.gridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.gridLength; j++)
            {
                gridNumber[i, j] = 0;
                puzzleNumber[i, j] = 0;
                puzzleBak[i, j] = 0;
            }
        }
    }
}
