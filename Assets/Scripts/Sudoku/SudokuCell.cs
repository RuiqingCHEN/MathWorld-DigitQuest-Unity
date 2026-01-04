using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    public Vector2Int coordinate;
    public SudokuSubGrid subGrid;
    int value = 0;
    public TMP_Text txtNumber;
    public Button btnNum;

    void Awake()
    {
        btnNum = GetComponent<Button>();
        txtNumber = GetComponentInChildren<TMP_Text>();
        btnNum.onClick.AddListener(ButtonClicked);
    }
    void ButtonClicked()
    {
        SudokuGameManager.Instance.ActivateInputButton(this);
    }

    public void SetCoordinate(int row, int col)
    {
        coordinate = new Vector2Int(row, col);
    }

    public void SetSubGridParent(SudokuSubGrid parent)
    {
        subGrid = parent;
    }

    public void InitValues(int value)
    {
        if (value != 0)
        {
            txtNumber.text = value.ToString();
            txtNumber.color = new Color32(119, 110, 101, 255);
            btnNum.enabled = false;
        }
        else
        {
            btnNum.enabled = true;
            txtNumber.text = " ";
            txtNumber.color = new Color32(0, 102, 187, 255);
        }
    }

    public void UpdateValue(int newValue)
    {
        value = newValue;

        if (value != 0)
        {
            txtNumber.text = value.ToString();
        }
        else
        {
            txtNumber.text = "";
        }
    }
}
