using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreBasedRewardConfig", menuName = "Game/Score Based Reward Config")]
public class ScoreBasedRewardConfig : ScriptableObject
{
    public string miniGameID;
    public List<ScoreRewardTier> scoreTiers = new List<ScoreRewardTier>();
}

[System.Serializable]
public class ScoreRewardTier
{
    public string tierName;
    public int scoreThreshold;
    public int goldAmount;
}

[CreateAssetMenu(fileName = "TimeBasedRewardConfig", menuName = "Game/Time Based Reward Config")]
public class TimeBasedRewardConfig : ScriptableObject
{
    public string miniGameID;
    public List<TimeRewardTier> timeTiers = new List<TimeRewardTier>();
}

[System.Serializable]
public class TimeRewardTier
{
    public string tierName;
    public float timeThreshold;
    public int bonusGold;
}