using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamesSceneController : MonoBehaviour
{
    [Header("������ ���")]
    [SerializeField] private Button[] gameButtons;

    [Header("������������� ������")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button okrPreparationButton; // ����� ������ ��� �������� � ���

    [Header("�������� ���")]
    [SerializeField]
    private string[] gameNames = {
        "������",
        "������",
        "������",
        "������",
        "������"
    };

    [Header("��������� ����")]
    [SerializeField] private string mainMenuName = "MainMenu";
    [SerializeField] private string okrPreparationSceneName = "PreparationScene"; // �������� ����� ���������� � ���

    private GameObject currentMessage; // ��� ������������ �������� ���������

    void Start()
    {
        for (int i = 0; i < Mathf.Min(gameButtons.Length, gameNames.Length); i++)
        {
            int index = i;
            gameButtons[i].onClick.AddListener(() => OnGameButtonClick(index));

            Text buttonText = gameButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = gameNames[i];
        }

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToMainMenu);

        if (okrPreparationButton != null)
            okrPreparationButton.onClick.AddListener(GoToOKRPreparation);

        ApplyRoleFilter();
    }

    private void ApplyRoleFilter()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        var role = SessionManager.Instance.Role;

        if (okrPreparationButton != null)
        {
            var tmpText = okrPreparationButton.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmpText != null)
                tmpText.text = ContentFilter.GetPreparationLabel(role);
            else
            {
                var legacyText = okrPreparationButton.GetComponentInChildren<Text>();
                if (legacyText != null)
                    legacyText.text = ContentFilter.GetPreparationLabel(role);
            }
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void ReturnToMainMenu()
    {
        // ������ �������� ����� ��� �������� ����������
        SceneManager.LoadScene(mainMenuName);
    }

    void GoToOKRPreparation()
    {
        Debug.Log($"������� � ���������� ���: {okrPreparationSceneName}");

        // ���������, ���������� �� ����� � ����� ������
        if (SceneExists(okrPreparationSceneName))
        {
            SceneManager.LoadScene(okrPreparationSceneName);
        }
        else
        {
            ShowErrorMessage($"����� '{okrPreparationSceneName}' �� �������!");
            Debug.LogError($"����� '{okrPreparationSceneName}' �� �������. ��������� �������� � Build Settings.");
        }
    }

    void OnGameButtonClick(int gameIndex)
    {
        if (gameIndex >= 0 && gameIndex < gameNames.Length)
        {
            Debug.Log($"������� ����: {gameNames[gameIndex]}");
            ShowGameMessage(gameNames[gameIndex]);
        }
    }

    void ShowGameMessage(string gameName)
    {
        // ������� ���������� ���������, ���� ��� ����
        if (currentMessage != null)
        {
            Destroy(currentMessage);
        }

        // ������� ���������
        GameObject message = new GameObject("Message");
        currentMessage = message; // ��������� ������ �� ������� ���������

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            message.transform.SetParent(canvas.transform);
        }

        // ������������� �� ������
        RectTransform rect = message.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400, 200);

        // ��������� ���
        Image background = message.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);

        // ��������� �����
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(message.transform);

        Text text = textObj.AddComponent<Text>();
        text.text = $"���� '{gameName}' � ����������!";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        // ����������� ����� �� ��� ������
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // ������� ����� 2 �������
        Destroy(message, 2f);
    }

    void ShowErrorMessage(string errorMessage)
    {
        // ������� ���������� ���������, ���� ��� ����
        if (currentMessage != null)
        {
            Destroy(currentMessage);
        }

        // ������� ��������� �� ������
        GameObject message = new GameObject("ErrorMessage");
        currentMessage = message;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            message.transform.SetParent(canvas.transform);
        }

        // ������������� �� ������
        RectTransform rect = message.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(500, 250);

        // ��������� ��� (������� ��� ������)
        Image background = message.AddComponent<Image>();
        background.color = new Color(0.8f, 0, 0, 0.9f);

        // ��������� �����
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(message.transform);

        Text text = textObj.AddComponent<Text>();
        text.text = errorMessage;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = 24;

        // ����������� ����� �� ��� ������
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        // ������� ����� 3 �������
        Destroy(message, 3f);
    }

    bool SceneExists(string sceneName)
    {
        // ���������, ���������� �� ����� � Build Settings
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

    void OnDestroy()
    {
        // ������� ������ ��� ����������� �������
        if (currentMessage != null)
        {
            Destroy(currentMessage);
        }
    }
}