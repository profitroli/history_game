using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Choice
{
    public string choiceAText;
    public string choiceBText;
    public int choiceAImpact = 1;
    public int choiceBImpact = -1;
}

[System.Serializable]
public class Stage
{
    public string stageName;
    [TextArea(2, 4)]
    public string stageIntroText; // �����, ������� ���������� � ������ �����
    public Sprite characterSprite;
    public Choice[] choices;
}

public class GameManager35 : MonoBehaviour
{
    public static GameManager35 Instance;

    public Stage[] stages;

    // ������ �� UI ��������
    public Image characterImage;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI stageIntroText; // ��������� ���� ��� �������������� ������ �����
    public TextMeshProUGUI feedbackText; // ���� ��� �������� �����
    public Button choiceButtonA;
    public Button choiceButtonB;
    public GameObject winPanel;
    public GameObject failPanel;

    private int currentStageIndex = 0;
    private int currentChoiceIndex = 0;
    private int stageProgress = 0;
    private int badChoicesCount = 0;
    private const int MAX_BAD_CHOICES = 5;
    private const int REQUIRED_PROGRESS = 4;

    // ����� ��� �������� �����
    private Color goodColor = Color.green;
    public Color badColor = Color.red;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        choiceButtonA.onClick.AddListener(() => MakeChoice(0));
        choiceButtonB.onClick.AddListener(() => MakeChoice(1));

        // �������������� ������
        feedbackText.text = "";
        stageIntroText.text = "";

        LoadCurrentStage();
    }

    void LoadCurrentStage()
    {
        if (currentStageIndex >= stages.Length)
        {
            ShowWinScreen();
            return;
        }

        var stage = stages[currentStageIndex];

        // ��������� UI
        characterImage.sprite = stage.characterSprite;
        stageText.text = $"���� {currentStageIndex + 1}: {stage.stageName}";
        progressText.text = $"�������� �����: {stageProgress}/{REQUIRED_PROGRESS}";

        // ���������� ������������� ����� �����
        ShowStageIntro(stage.stageIntroText);

        // ��������� ������� �����
        LoadCurrentChoice();
    }

    void ShowStageIntro(string introText)
    {
        stageIntroText.text = introText;
        stageIntroText.color = Color.blue; // ����� ���� ��� �������������� ������

        // ������������� �������� ����� 3 �������
        StartCoroutine(HideStageIntroAfterDelay(3f));
    }

    IEnumerator HideStageIntroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ������ �������� �����
        float fadeDuration = 0.2f;
        float elapsedTime = 0f;
        Color startColor = stageIntroText.color;
        Color transparentColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            stageIntroText.color = Color.Lerp(startColor, transparentColor, elapsedTime / fadeDuration);
            yield return null;
        }

        stageIntroText.text = "";
    }

    void LoadCurrentChoice()
    {
        var stage = stages[currentStageIndex];

        if (currentChoiceIndex >= stage.choices.Length)
            currentChoiceIndex = stage.choices.Length - 1;

        var choice = stage.choices[currentChoiceIndex];

        var textA = choiceButtonA.GetComponentInChildren<TextMeshProUGUI>();
        var textB = choiceButtonB.GetComponentInChildren<TextMeshProUGUI>();
        textA.text = choice.choiceAText;
        textB.text = choice.choiceBText;
    }

    void MakeChoice(int choiceType)
    {
        var stage = stages[currentStageIndex];
        var choice = stage.choices[currentChoiceIndex];

        int impact = (choiceType == 0) ? choice.choiceAImpact : choice.choiceBImpact;

        if (impact < 0) // ������ �����
        {
            ShowFeedback("�������!", badColor);
            badChoicesCount++;
            if (badChoicesCount >= MAX_BAD_CHOICES)
            {
                ShowFailScreen();
                return;
            }

            // ����� �� ���������� �����
            if (currentChoiceIndex > 0)
                currentChoiceIndex--;

            // �������� ��������
            stageProgress = Mathf.Max(0, stageProgress - 1);
        }
        else // ������� �����
        {
            stageProgress += impact;

            // ��������� � ���������� ������
            if (currentChoiceIndex < stage.choices.Length - 1)
                currentChoiceIndex++;

            // ��������� ���������� �����
            if (stageProgress >= REQUIRED_PROGRESS)
            {
                // ��������� �� ��������� ����
                currentStageIndex++;
                currentChoiceIndex = 0;
                stageProgress = 0;
                badChoicesCount = 0;

                LoadCurrentStage();
                return;
            }
        }

        progressText.text = $"�������� �����: {stageProgress}/{REQUIRED_PROGRESS}";
        LoadCurrentChoice();
    }

    void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;

        StartCoroutine(HideFeedbackAfterDelay(2f));
    }

    IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        Color startColor = feedbackText.color;
        Color transparentColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            feedbackText.color = Color.Lerp(startColor, transparentColor, elapsedTime / fadeDuration);
            yield return null;
        }

        feedbackText.text = "";
    }

    void ShowWinScreen()
    {
        ProgressService.Instance?.ReportGameCompleted(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        winPanel.SetActive(true);
        choiceButtonA.gameObject.SetActive(false);
        choiceButtonB.gameObject.SetActive(false);
    }

    void ShowFailScreen()
    {
        failPanel.SetActive(true);
        choiceButtonA.gameObject.SetActive(false);
        choiceButtonB.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        currentStageIndex = 0;
        currentChoiceIndex = 0;
        stageProgress = 0;
        badChoicesCount = 0;

        winPanel.SetActive(false);
        failPanel.SetActive(false);
        choiceButtonA.gameObject.SetActive(true);
        choiceButtonB.gameObject.SetActive(true);

        feedbackText.text = "";
        stageIntroText.text = "";

        LoadCurrentStage();
    }
}