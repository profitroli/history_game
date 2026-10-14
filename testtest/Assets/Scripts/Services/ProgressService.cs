using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressService : MonoBehaviour
{
    public static ProgressService Instance { get; private set; }

    private ProgressData currentProgress = new ProgressData();
    private bool isDirty;
    private Coroutine saveCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ProgressData GetProgress() => currentProgress;

    public void ReportLectureViewed(string lectureName)
    {
        if (string.IsNullOrEmpty(lectureName)) return;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        currentProgress.lectures[lectureName] = timestamp;
        MarkDirty();
        Debug.Log($"[ProgressService] Лекция просмотрена: {lectureName}");
    }

    public void ReportGameCompleted(string gameName, int score)
    {
        if (string.IsNullOrEmpty(gameName)) return;
        currentProgress.games[gameName] = new GameProgress
        {
            score = score,
            completedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        MarkDirty();
        Debug.Log($"[ProgressService] Игра пройдена: {gameName}, счёт: {score}");
    }

    public void ReportTestCompleted(string testName, int score, int total)
    {
        if (string.IsNullOrEmpty(testName)) return;
        currentProgress.tests[testName] = new TestProgress
        {
            score = score,
            total = total,
            completedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        MarkDirty();
        Debug.Log($"[ProgressService] Тест пройден: {testName}, {score}/{total}");
    }

    public async void LoadFromFirestore()
    {
        if (SessionManager.Instance == null || SessionManager.Instance.IsSelfStudy)
        {
            LoadFromPlayerPrefs();
            return;
        }

        try
        {
            var doc = await FirebaseManager.Instance.Service.GetDocument("progress", SessionManager.Instance.UserId);
            if (doc != null)
            {
                DeserializeProgress(doc);
                Debug.Log("[ProgressService] Прогресс загружен из Firestore");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ProgressService] Ошибка загрузки: {e.Message}");
            LoadFromPlayerPrefs();
        }
    }

    private void MarkDirty()
    {
        isDirty = true;
        if (saveCoroutine == null)
            saveCoroutine = StartCoroutine(DebouncedSave());
    }

    private IEnumerator DebouncedSave()
    {
        yield return new WaitForSeconds(2f);
        if (isDirty)
        {
            isDirty = false;
            SaveProgress();
        }
        saveCoroutine = null;
    }

    private async void SaveProgress()
    {
        SaveToPlayerPrefs();

        if (SessionManager.Instance == null || SessionManager.Instance.IsSelfStudy)
            return;

        try
        {
            var data = SerializeProgress();
            await FirebaseManager.Instance.Service.SetDocument("progress", SessionManager.Instance.UserId, data);
            Debug.Log("[ProgressService] Прогресс сохранён в Firestore");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ProgressService] Ошибка сохранения в Firestore: {e.Message}");
        }
    }

    private Dictionary<string, object> SerializeProgress()
    {
        var lecturesMap = new Dictionary<string, object>();
        foreach (var kv in currentProgress.lectures)
            lecturesMap[kv.Key] = kv.Value;

        var gamesMap = new Dictionary<string, object>();
        foreach (var kv in currentProgress.games)
        {
            var gameMap = new Dictionary<string, object>
            {
                { "score", kv.Value.score },
                { "completedAt", kv.Value.completedAt }
            };
            gamesMap[kv.Key] = gameMap;
        }

        var testsMap = new Dictionary<string, object>();
        foreach (var kv in currentProgress.tests)
        {
            var testMap = new Dictionary<string, object>
            {
                { "score", kv.Value.score },
                { "total", kv.Value.total },
                { "completedAt", kv.Value.completedAt }
            };
            testsMap[kv.Key] = testMap;
        }

        return new Dictionary<string, object>
        {
            { "lectures", lecturesMap },
            { "games", gamesMap },
            { "tests", testsMap }
        };
    }

    private void DeserializeProgress(Dictionary<string, object> doc)
    {
        currentProgress = new ProgressData();
        // TODO: парсинг вложенных map-полей из Firestore
    }

    private void SaveToPlayerPrefs()
    {
        string json = JsonUtility.ToJson(new ProgressWrapper
        {
            lectureKeys = new List<string>(currentProgress.lectures.Keys),
            gameKeys = new List<string>(currentProgress.games.Keys),
            testKeys = new List<string>(currentProgress.tests.Keys)
        });
        PlayerPrefs.SetString("LocalProgress", json);
        PlayerPrefs.Save();
    }

    private void LoadFromPlayerPrefs()
    {
        string json = PlayerPrefs.GetString("LocalProgress", "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var wrapper = JsonUtility.FromJson<ProgressWrapper>(json);
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                foreach (var key in wrapper.lectureKeys)
                    currentProgress.lectures[key] = now;
                foreach (var key in wrapper.gameKeys)
                    currentProgress.games[key] = new GameProgress { score = 1, completedAt = now };
                foreach (var key in wrapper.testKeys)
                    currentProgress.tests[key] = new TestProgress { score = 0, total = 0, completedAt = now };
            }
            catch { }
        }
    }

    void OnApplicationQuit()
    {
        if (isDirty)
        {
            isDirty = false;
            SaveToPlayerPrefs();
        }
    }

    [Serializable]
    private class ProgressWrapper
    {
        public List<string> lectureKeys = new List<string>();
        public List<string> gameKeys = new List<string>();
        public List<string> testKeys = new List<string>();
    }
}
