using System;
using System.Collections.Generic;

[Serializable]
public class ProgressData
{
    public Dictionary<string, long> lectures = new Dictionary<string, long>();
    public Dictionary<string, GameProgress> games = new Dictionary<string, GameProgress>();
    public Dictionary<string, TestProgress> tests = new Dictionary<string, TestProgress>();

    public int LecturesCompleted => lectures.Count;
    public int GamesCompleted => games.Count;
    public int TestsCompleted => tests.Count;

    public float AverageTestScore
    {
        get
        {
            if (tests.Count == 0) return 0f;
            float total = 0f;
            foreach (var t in tests.Values)
                total += (float)t.score / t.total;
            return total / tests.Count * 100f;
        }
    }
}

[Serializable]
public class GameProgress
{
    public int score;
    public long completedAt;
}

[Serializable]
public class TestProgress
{
    public int score;
    public int total;
    public long completedAt;
}
