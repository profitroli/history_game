using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // �������� ����� ����������
    public const string MAIN_MENU = "MainMenu";
    public const string LECTURES_SCENE = "LecturesScene";
    public const string GAMES_SCENE = "GamesScene";
    public const string PREPARATION_SCENE = "PreparationScene";
    public const string SETTINGS_SCENE = "SettingsScene";
    public const string LOGIN_SCENE = "LoginScene";
    public const string TEACHER_DASHBOARD = "TeacherDashboardScene";

    // �������� ����� ��� �������� ����
    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("��� ����� �� ����� ���� ������!");
            return;
        }

        // ��������� ������������� �����
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"����� '{sceneName}' �� ������� � Build Settings!");
            LoadMainMenu(); // ���������� � ������� ���� ��� ������
            return;
        }

        // ��������� �����
        SceneManager.LoadScene(sceneName);
    }

    // ������ ��� ������� �������� ���������� ����
    public static void LoadMainMenu()
    {
        LoadScene(MAIN_MENU);
    }

    public static void LoadLecturesScene()
    {
        LoadScene(LECTURES_SCENE);
    }

    public static void LoadGamesScene()
    {
        LoadScene(GAMES_SCENE);
    }

    public static void LoadPreparationScene()
    {
        LoadScene(PREPARATION_SCENE);
    }

    public static void LoadSettingsScene()
    {
        LoadScene(SETTINGS_SCENE);
    }

    // ����� ��� �������� ������� ����� � Build Settings
    private static bool IsSceneInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            // �������� ���� � ����� � ��������� ���
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string nameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (nameFromPath == sceneName)
            {
                return true;
            }
        }

        return false;
    }
    // �������������� �������� ������
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static void ReloadCurrentScene()
    {
        string currentScene = GetCurrentSceneName();
        LoadScene(currentScene);
    }

    public static void LoadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("��� ����� �� ����� ���� ������!");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"����� '{sceneName}' �� ������� � Build Settings!");
            return;
        }

        SceneManager.LoadSceneAsync(sceneName);
    }
}