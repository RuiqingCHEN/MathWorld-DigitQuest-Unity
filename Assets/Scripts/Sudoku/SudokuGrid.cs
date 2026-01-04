using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public SudokuSubGrid[,] subGrids { get; private set; }
    public SudokuCell[] cells;

    void Awake()
    {
        var grid = GetComponentsInChildren<SudokuSubGrid>();

        subGrids = new SudokuSubGrid[SudokuGameManager.subGridLength, SudokuGameManager.subGridLength];

        int index = 0;
        for (int i = 0; i < SudokuGameManager.subGridLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.subGridLength; j++)
            {
                subGrids[i, j] = grid[index++];
                subGrids[i, j].SetCoordinate(i, j);
                subGrids[i, j].InitCells();
            }
        }
        cells = GetComponentsInChildren<SudokuCell>();
    }

    public SudokuCell GetCellByPosition(int row, int col)
    {
        int subGridRow = row / SudokuGameManager.subGridLength;
        int subGridCol = col / SudokuGameManager.subGridLength;
        int cellRow = row % SudokuGameManager.subGridLength;
        int cellCol = col % SudokuGameManager.subGridLength;

        if (subGridRow < SudokuGameManager.subGridLength && subGridCol < SudokuGameManager.subGridLength && subGrids[subGridRow, subGridCol] != null)
        {
            return subGrids[subGridRow, subGridCol].cells[cellRow, cellCol];
        }

        return null;
    }
}
