using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private float fadeTime = 2f;

    [Header("������ �� �������� UI")]
    [SerializeField] private Button lecturesButton;
    [SerializeField] private Button miniGamesButton;
    [SerializeField] private Button preparationButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private CanvasGroup menuCanvasGroup;

    [Header("����� ��� ��������")]
    [SerializeField] private string lecturesSceneName = "LecturesScene";
    [SerializeField] private string miniGamesSceneName = "GamesScene";
    [SerializeField] private string preparationSceneName = "PreparationScene";
    [SerializeField] private string settingsSceneName = "SettingsScene";

    // ����������� ���������� ��� ������������ ���������
    private static bool isFirstTime = true;
    private static bool shouldKeepBlackScreen = false;

    void Awake()
    {
        Debug.Log("MainMenu Awake - isFirstTime: " + isFirstTime);

        if (isFirstTime)
        {
            // ������ ��� - ��������� ������ ����� ��� ��������
            isFirstTime = false;
            shouldKeepBlackScreen = true;
            Debug.Log("��� ������ ������, ��������� ������ ����� ��� ��������");
        }
        else
        {
            // �� ������ ��� - ������� ������ ����� �����
            shouldKeepBlackScreen = false;
            DestroyAllBlackScreensImmediately();
            Debug.Log("�� ������ ������, ������� ������ ����� �����");
        }
    }

    void Start()
    {
        Debug.Log("MainMenu Start - shouldKeepBlackScreen: " + shouldKeepBlackScreen);

        if (shouldKeepBlackScreen)
        {
            // ���� ����� �������� ������ ����� ��� ��������
            StartCoroutine(ShowMenuWithFade());
            shouldKeepBlackScreen = false; // ����� �������� ������ �� �����
        }
        else
        {
            // ���� ������ ����� ��� ������ ��� �� �����
            ShowMenuInstantly();
        }

        InitializeButtons();
        WireSectionCards();
    }

    IEnumerator ShowMenuWithFade()
    {
        Debug.Log("�������� �������� � ������ �������");

        // ���� ������ �����
        GameObject blackScreen = GameObject.Find("FadeImage");
        if (blackScreen == null)
        {
            blackScreen = GameObject.Find("BlackScreenCanvas");
        }

        // ���� ������ Image
        Image blackScreenImage = null;
        if (blackScreen != null)
        {
            blackScreenImage = blackScreen.GetComponent<Image>();
            if (blackScreenImage == null)
            {
                blackScreenImage = blackScreen.GetComponentInChildren<Image>();
            }
        }

        // �������� ���� � ������
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 0;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }

        // ���� ��������� �����
        yield return new WaitForSeconds(0.5f);

        float timer = 0f;

        while (timer < fadeTime)
        {
            float progress = timer / fadeTime;

            // ������ ������� ������ �����
            if (blackScreenImage != null)
            {
                blackScreenImage.color = new Color(0, 0, 0, 1 - progress);
            }

            // ������ ���������� ����
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = progress;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // ��������� ��������
        if (blackScreen != null)
        {
            Destroy(blackScreen);
            Debug.Log("������ ����� ������ ����� ��������");
        }

        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 1;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }

        Debug.Log("�������� ���������, ���� ��������");
    }

    private void DestroyAllBlackScreensImmediately()
    {
        // ������� ��� ��������� ������ ������
        GameObject[] blackScreenNames = {
            GameObject.Find("FadeImage"),
            GameObject.Find("BlackScreenCanvas"),
            GameObject.Find("BlackScreen")
        };

        foreach (GameObject obj in blackScreenNames)
        {
            if (obj != null)
            {
                Destroy(obj);
                Debug.Log("������ ������ �����: " + obj.name);
            }
        }

        // ���� ��� ������ �����������
        Image[] allImages = FindObjectsOfType<Image>(true);
        foreach (Image img in allImages)
        {
            if (img.color == Color.black && img.gameObject.name.Contains("Black"))
            {
                Destroy(img.gameObject);
                Debug.Log("������� ������ �����������: " + img.name);
            }
        }
    }

    private void InitializeButtons()
    {
        // ��������� ����������� ��� ������
        lecturesButton?.onClick.AddListener(LoadLecturesScene);
        miniGamesButton?.onClick.AddListener(LoadMiniGamesScene);
        preparationButton?.onClick.AddListener(LoadPreparationScene);
        exitButton?.onClick.AddListener(OnExitClick);
        settingsButton?.onClick.AddListener(LoadSettingsScene);

        ApplyRoleFilter();
    }

    private void ApplyRoleFilter()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;

        var role = SessionManager.Instance.Role;
        var prepText = preparationButton?.GetComponentInChildren<TMPro.TMP_Text>();
        if (prepText != null)
            prepText.text = ContentFilter.GetPreparationLabel(role);
        else
        {
            var legacyText = preparationButton?.GetComponentInChildren<Text>();
            if (legacyText != null)
                legacyText.text = ContentFilter.GetPreparationLabel(role);
        }
    }

    private void ShowMenuInstantly()
    {
        Debug.Log("���������� ����������� ����");

        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 1;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
    }

    // ����������� ����� ��� �������� �������� ���� �� ������ ����
    public static void LoadMainMenuFromOtherScene()
    {
        Debug.Log("�������� �������� ���� �� ������ �����");
        SceneManager.LoadScene("MainMenu");
    }

    // ������ �������� ����
    private void LoadLecturesScene()
    {
        LoadSceneWithValidation(lecturesSceneName, "������");
    }

    private void LoadMiniGamesScene()
    {
        LoadSceneWithValidation(miniGamesSceneName, "����-���");
    }

    private void LoadPreparationScene()
    {
        LoadSceneWithValidation(preparationSceneName, "���������� � ���");
    }

    public void LoadSettingsScene()
    {
        LoadSceneWithValidation(settingsSceneName, "��������");
    }

    private void LoadSceneWithValidation(string sceneName, string sceneDescription)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"��� ����� {sceneDescription} �� �����������!");
            ShowErrorMessage($"����� {sceneDescription} �� ���������");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"����� '{sceneName}' �� ������� � Build Settings!");
            ShowErrorMessage($"����� '{sceneName}' �� �������.\n�������� � � Build Settings.");
            return;
        }

        Debug.Log($"�������� �����: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // �������� ������������� ����� � Build Settings
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    // �������� ��������� �� ������
    private void ShowErrorMessage(string message)
    {
        GameObject errorObj = new GameObject("ErrorMessage");
        var canvasGo = GameObject.Find("Canvas");
        Canvas canvas = canvasGo != null ? canvasGo.GetComponent<Canvas>() : FindObjectOfType<Canvas>();

        if (canvas != null)
        {
            errorObj.transform.SetParent(canvas.transform);
            errorObj.transform.SetAsLastSibling();
        }

        RectTransform rect = errorObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 200);
        rect.anchoredPosition = Vector2.zero;

        Image bg = errorObj.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(errorObj.transform);

        Text text = textObj.AddComponent<Text>();
        text.text = message;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        Destroy(errorObj, 3f);
    }

    private void WireSectionCards()
    {
        Canvas canvasObj = GetComponentInParent<Canvas>();
        if (canvasObj == null)
        {
            var go = GameObject.Find("Canvas");
            if (go != null) canvasObj = go.GetComponent<Canvas>();
        }
        if (canvasObj == null) canvasObj = FindObjectOfType<Canvas>();
        if (canvasObj == null) return;

        var contentArea = canvasObj.transform.Find("ContentArea");
        if (contentArea == null) contentArea = canvasObj.transform.Find("SectionsPanel");
        if (contentArea == null) return;
        var homeView = contentArea.Find("HomeView");
        var cardsRow = homeView != null ? homeView.Find("CardsRow") : contentArea.Find("CardsRow");
        if (cardsRow == null) return;

        System.Action[] actions = { LoadLecturesScene, LoadMiniGamesScene, LoadPreparationScene, OpenProfilePanel };

        for (int i = 0; i < cardsRow.childCount && i < actions.Length; i++)
        {
            var card = cardsRow.GetChild(i);
            var btn = card.GetComponentInChildren<Button>();
            if (btn == null) continue;
            int idx = i;
            btn.onClick.AddListener(() => actions[idx]());
        }
    }

    private void OpenProfilePanel()
    {
        var uiController = FindObjectOfType<MainMenuUIController>();
        if (uiController != null) uiController.OpenProfile();
    }

    private void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        lecturesButton?.onClick.RemoveAllListeners();
        miniGamesButton?.onClick.RemoveAllListeners();
        preparationButton?.onClick.RemoveAllListeners();
        exitButton?.onClick.RemoveAllListeners();
        settingsButton?.onClick.RemoveAllListeners();
    }
}