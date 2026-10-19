using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssignmentPlayerController : MonoBehaviour
{
    [Header("Список заданий")]
    [SerializeField] private GameObject listPanel;
    [SerializeField] private Transform listContent;
    [SerializeField] private Button closeListButton;
    [SerializeField] private TMP_Text listTitle;

    [Header("Прохождение")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionNumberText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private Button optionCButton;
    [SerializeField] private Button optionDButton;
    [SerializeField] private TMP_Text optionAText;
    [SerializeField] private TMP_Text optionBText;
    [SerializeField] private TMP_Text optionCText;
    [SerializeField] private TMP_Text optionDText;

    [Header("Результат")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text resultScoreText;
    [SerializeField] private Button resultCloseButton;

    private AssignmentData currentAssignment;
    private int currentQuestion;
    private int score;
    private Color normalBtnColor = new Color(0.25f, 0.48f, 0.85f);
    private Color correctColor = new Color(0.2f, 0.75f, 0.3f);
    private Color wrongColor = new Color(0.85f, 0.25f, 0.25f);

    void Start()
    {
        if (closeListButton != null) closeListButton.onClick.AddListener(CloseAndReturnToProfile);
        if (resultCloseButton != null) resultCloseButton.onClick.AddListener(ShowList);

        if (optionAButton != null) optionAButton.onClick.AddListener(() => OnAnswer(0));
        if (optionBButton != null) optionBButton.onClick.AddListener(() => OnAnswer(1));
        if (optionCButton != null) optionCButton.onClick.AddListener(() => OnAnswer(2));
        if (optionDButton != null) optionDButton.onClick.AddListener(() => OnAnswer(3));
    }

    void OnEnable()
    {
        ShowList();
    }

    private void ShowList()
    {
        if (listPanel != null) listPanel.SetActive(true);
        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
        LoadAssignments();
    }

    private async void LoadAssignments()
    {
        if (listContent == null) return;

        foreach (Transform child in listContent)
            Destroy(child.gameObject);

        string groupId = SessionManager.Instance?.GroupId;
        if (string.IsNullOrEmpty(groupId))
        {
            AddMessage(listContent, "Вы не состоите в группе");
            return;
        }

        if (listTitle != null) listTitle.text = "Задания от преподавателя";

        var assignments = await AssignmentService.GetGroupAssignments(groupId, activeOnly: true);
        if (assignments.Count == 0)
        {
            AddMessage(listContent, "Нет доступных заданий");
            return;
        }

        foreach (var a in assignments)
        {
            var row = new GameObject("Assign_" + a.assignmentId);
            row.transform.SetParent(listContent, false);
            var rt = row.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600, 55);
            row.AddComponent<Image>().color = new Color(0.93f, 0.93f, 0.96f);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(15, 15, 8, 8);
            layout.childForceExpandWidth = false;

            var nameGO = new GameObject("Name");
            nameGO.transform.SetParent(row.transform, false);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = $"{a.name}\n<size=12><color=#888>{a.questions.Count} вопр. • {a.teacherName}</color></size>";
            nameTMP.fontSize = 16;
            nameTMP.richText = true;
            var nameLE = nameGO.AddComponent<LayoutElement>();
            nameLE.preferredWidth = 400;
            nameLE.flexibleWidth = 1;

            string capturedId = a.assignmentId;
            var btnGO = new GameObject("PlayBtn");
            btnGO.transform.SetParent(row.transform, false);
            btnGO.AddComponent<Image>().color = normalBtnColor;
            var btn = btnGO.AddComponent<Button>();
            var btnLE = btnGO.AddComponent<LayoutElement>();
            btnLE.preferredWidth = 120;
            btnLE.preferredHeight = 38;
            var btnTxtGO = new GameObject("T");
            btnTxtGO.transform.SetParent(btnGO.transform, false);
            var btnTxt = btnTxtGO.AddComponent<TextMeshProUGUI>();
            btnTxt.text = "Начать";
            btnTxt.fontSize = 15;
            btnTxt.color = Color.white;
            btnTxt.alignment = TextAlignmentOptions.Center;
            var bRT = btnTxtGO.GetComponent<RectTransform>();
            bRT.anchorMin = Vector2.zero;
            bRT.anchorMax = Vector2.one;
            bRT.offsetMin = bRT.offsetMax = Vector2.zero;

            btn.onClick.AddListener(() => StartQuiz(capturedId));
        }
    }

    private async void StartQuiz(string assignmentId)
    {
        currentAssignment = await AssignmentService.GetAssignment(assignmentId);
        if (currentAssignment == null || currentAssignment.questions.Count == 0) return;

        currentQuestion = 0;
        score = 0;

        if (listPanel != null) listPanel.SetActive(false);
        if (quizPanel != null) quizPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentQuestion >= currentAssignment.questions.Count)
        {
            ShowResult();
            return;
        }

        var q = currentAssignment.questions[currentQuestion];

        if (questionNumberText != null)
            questionNumberText.text = $"Вопрос {currentQuestion + 1} из {currentAssignment.questions.Count}";
        if (questionText != null)
            questionText.text = q.text;

        SetOptionText(optionAText, $"А) {q.optionA}");
        SetOptionText(optionBText, $"Б) {q.optionB}");
        SetOptionText(optionCText, $"В) {q.optionC}");
        SetOptionText(optionDText, $"Г) {q.optionD}");

        SetButtonColor(optionAButton, normalBtnColor);
        SetButtonColor(optionBButton, normalBtnColor);
        SetButtonColor(optionCButton, normalBtnColor);
        SetButtonColor(optionDButton, normalBtnColor);

        EnableButtons(true);
    }

    private void OnAnswer(int selected)
    {
        EnableButtons(false);

        var q = currentAssignment.questions[currentQuestion];
        bool correct = selected == q.correctIndex;
        if (correct) score++;

        Button selectedBtn = GetButton(selected);
        Button correctBtn = GetButton(q.correctIndex);

        if (correct)
        {
            SetButtonColor(selectedBtn, correctColor);
        }
        else
        {
            SetButtonColor(selectedBtn, wrongColor);
            SetButtonColor(correctBtn, correctColor);
        }

        Invoke(nameof(NextQuestion), 1.2f);
    }

    private void NextQuestion()
    {
        currentQuestion++;
        ShowQuestion();
    }

    private async void ShowResult()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);

        int total = currentAssignment.questions.Count;
        float pct = total > 0 ? (float)score / total * 100f : 0;

        if (resultText != null)
            resultText.text = $"Задание «{currentAssignment.name}» завершено!";
        if (resultScoreText != null)
        {
            string level = pct >= 80 ? "Отлично!" : pct >= 50 ? "Хорошо" : "Попробуй ещё";
            resultScoreText.text = $"{score}/{total} ({pct:F0}%)\n{level}";
            resultScoreText.color = pct >= 80 ? correctColor : pct >= 50 ? new Color(0.9f, 0.75f, 0.1f) : wrongColor;
        }

        await AssignmentService.SaveAssignmentResult(
            currentAssignment.assignmentId, currentAssignment.name, score, total);
    }

    private void AddMessage(Transform parent, string msg)
    {
        var go = new GameObject("Msg");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 40);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = msg;
        tmp.fontSize = 16;
        tmp.color = Color.gray;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    private void SetOptionText(TMP_Text t, string s) { if (t != null) t.text = s; }
    private void SetButtonColor(Button b, Color c) { if (b != null) b.GetComponent<Image>().color = c; }
    private void EnableButtons(bool e)
    {
        if (optionAButton != null) optionAButton.interactable = e;
        if (optionBButton != null) optionBButton.interactable = e;
        if (optionCButton != null) optionCButton.interactable = e;
        if (optionDButton != null) optionDButton.interactable = e;
    }

    private void CloseAndReturnToProfile()
    {
        gameObject.SetActive(false);
        var uiCtrl = Object.FindFirstObjectByType<MainMenuUIController>();
        if (uiCtrl != null) uiCtrl.ReturnToProfile();
    }

    private Button GetButton(int idx)
    {
        switch (idx)
        {
            case 0: return optionAButton;
            case 1: return optionBButton;
            case 2: return optionCButton;
            case 3: return optionDButton;
            default: return null;
        }
    }
}
