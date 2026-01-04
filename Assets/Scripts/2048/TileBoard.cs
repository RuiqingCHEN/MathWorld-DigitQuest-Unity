using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager2048 gameManager2048;
    public Tile tilePrefab;
    public TileState[] tileStates;
    public TileGrid grid;
    public List<Tile> tiles = new List<Tile>();
    private bool waiting = false;

    public CanvasGroup gameCanvasGroup;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);

        tile.SetState(tileStates[0]);

        TileCell cell = grid.GetRandomEmptyCell();
        if (cell != null)
        {
            tile.LinkCell(cell);
            tiles.Add(tile);
        }
        else
        {
            Destroy(tile.gameObject);
        }



    }

    private void Update()
    {
        if (gameCanvasGroup != null && gameCanvasGroup.alpha < 1f) return;

        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }

    void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int X = startX; X >= 0 && X < grid.width; X += incrementX)
        {
            for (int Y = startY; Y >= 0 && Y < grid.height; Y += incrementY)
            {
                TileCell cell = grid.GetCell(X, Y);

                if (cell.IsOccupied())
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.IsOccupied())
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
        gameManager2048.IncreaseScore(newState.TileNumber);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count < grid.size)
        {
            CreateTile();
        }

        if (CheckForGameOver())
        {
            gameManager2048.GameOver();
        }
    }

    bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && up.IsOccupied() && CanMerge(tile, up.tile))
            {
                return false;
            }

            if (down != null && down.IsOccupied() && CanMerge(tile, down.tile))
            {
                return false;
            }

            if (left != null && left.IsOccupied() && CanMerge(tile, left.tile))
            {
                return false;
            }

            if (right != null && right.IsOccupied() && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }
}
