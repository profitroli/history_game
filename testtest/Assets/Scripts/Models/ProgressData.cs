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
            {
                if (t.total > 0)
                    total += (float)t.score / t.total;
            }
            return total / tests.Count * 100f;
        }
    }

    public static ProgressData FromFirestoreDoc(Dictionary<string, object> doc)
    {
        var p = new ProgressData();
        if (doc == null) return p;

        if (doc.ContainsKey("lectures") && doc["lectures"] is Dictionary<string, object> lecMap)
        {
            foreach (var kv in lecMap)
                p.lectures[kv.Key] = ToLong(kv.Value);
        }

        if (doc.ContainsKey("games") && doc["games"] is Dictionary<string, object> gamesMap)
        {
            foreach (var kv in gamesMap)
            {
                if (kv.Value is Dictionary<string, object> gm)
                {
                    p.games[kv.Key] = new GameProgress
                    {
                        score = ToInt(gm.ContainsKey("score") ? gm["score"] : null),
                        completedAt = ToLong(gm.ContainsKey("completedAt") ? gm["completedAt"] : null)
                    };
                }
            }
        }

        if (doc.ContainsKey("tests") && doc["tests"] is Dictionary<string, object> testsMap)
        {
            foreach (var kv in testsMap)
            {
                if (kv.Value is Dictionary<string, object> tm)
                {
                    p.tests[kv.Key] = new TestProgress
                    {
                        score = ToInt(tm.ContainsKey("score") ? tm["score"] : null),
                        total = ToInt(tm.ContainsKey("total") ? tm["total"] : null),
                        completedAt = ToLong(tm.ContainsKey("completedAt") ? tm["completedAt"] : null)
                    };
                }
            }
        }

        return p;
    }

    private static long ToLong(object val)
    {
        if (val is long l) return l;
        if (val is int i) return i;
        if (val is string s && long.TryParse(s, out long parsed)) return parsed;
        return 0;
    }

    private static int ToInt(object val)
    {
        if (val is long l) return (int)l;
        if (val is int i) return i;
        if (val is string s && int.TryParse(s, out int parsed)) return parsed;
        return 0;
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
