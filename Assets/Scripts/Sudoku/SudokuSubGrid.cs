using UnityEngine;

public class SudokuSubGrid : MonoBehaviour
{
    public Vector2Int coordinate;
    public SudokuCell[,] cells { get; private set; }

    private void Awake()
    {
        cells = new SudokuCell[SudokuGameManager.cellLength, SudokuGameManager.cellLength];
    }

    public void SetCoordinate(int row, int col)
    {
        coordinate = new Vector2Int(row, col);
    }

    public void InitCells()
    {
        for (int i = 0; i < SudokuGameManager.cellLength; i++)
        {
            for (int j = 0; j < SudokuGameManager.cellLength; j++)
            {
                cells[i, j] = Instantiate(SudokuGameManager.Instance.SudokuCell_Prefab, transform);
                cells[i, j].SetCoordinate(coordinate.x * SudokuGameManager.cellLength + i, coordinate.y * SudokuGameManager.cellLength + j);
                cells[i, j].SetSubGridParent(this);
                cells[i, j].InitValues(1);
            }
        }
    }
}
