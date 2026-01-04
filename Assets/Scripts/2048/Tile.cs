using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public TileState state;

    [HideInInspector]
    public TileCell cell;

    [HideInInspector]
    public bool locked;

    public Image imgBackground;
    public TMP_Text txtNumber;

    public void SetState(TileState state)
    {
        this.state = state;

        imgBackground.color = state.TileBackColor;
        txtNumber.color = state.TileTextColor;
        txtNumber.text = state.TileNumber.ToString();
    }

    public void LinkCell(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(MoveAnimate(cell.transform.position, true));
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        cell.tile = this;

        StartCoroutine(MoveAnimate(cell.transform.position, false));
    }

    IEnumerator MoveAnimate(Vector3 posTo, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 posFrom = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(posFrom, posTo, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = posTo;

        if (merging)
        {
            Destroy(gameObject);
        }
    }

}
