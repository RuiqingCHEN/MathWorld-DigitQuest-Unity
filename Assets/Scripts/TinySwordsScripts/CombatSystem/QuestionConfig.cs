using UnityEngine;

[CreateAssetMenu(menuName = "Quiz/QuestionConfig")]
public class QuestionConfig : ScriptableObject
{
    public OperationType operationType;
    public int tier;
    public int minNumber;
    public int maxNumber;
    public bool allowCarrying = true;
}

public enum OperationType { Addition, Subtraction, Multiplication, Division }
public enum QuestionTier { Tier1, Tier2, Tier3 }