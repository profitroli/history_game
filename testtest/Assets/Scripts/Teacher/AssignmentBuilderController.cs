using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssignmentBuilderController : MonoBehaviour
{
    [Header("Список заданий")]
    [SerializeField] private GameObject assignmentListPanel;
    [SerializeField] private Transform assignmentListContent;
    [SerializeField] private Button newAssignmentButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text assignmentListTitle;

    [Header("Редактор задания")]
    [SerializeField] private GameObject editorPanel;
    [SerializeField] private TMP_InputField assignmentNameInput;
    [SerializeField] private Transform questionsContent;
    [SerializeField] private Button addQuestionButton;
    [SerializeField] private Button saveAssignmentButton;
    [SerializeField] private Button cancelEditorButton;
    [SerializeField] private TMP_Text editorStatusText;

    private string currentGroupId;
    private string currentGroupName;
    private List<AssignmentData> assignments = new List<AssignmentData>();
    private List<QuestionEntry> questionEntries = new List<QuestionEntry>();

    private class QuestionEntry
    {
        public GameObject go;
        public int questionType;
        public TMP_InputField textInput;
        public TMP_InputField optAInput;
        public TMP_InputField optBInput;
        public TMP_InputField optCInput;
        public TMP_InputField optDInput;
        public TMP_Dropdown correctDropdown;
        public TMP_InputField correctTextInput;
    }

    public void Show(string groupId, string groupName)
    {
        currentGroupId = groupId;
        currentGroupName = groupName;
        gameObject.SetActive(true);

        if (assignmentListPanel != null) assignmentListPanel.SetActive(true);
        if (editorPanel != null) editorPanel.SetActive(false);

        if (assignmentListTitle != null)
            assignmentListTitle.text = $"Задания: {groupName}";

        if (newAssignmentButton != null)
        {
            newAssignmentButton.onClick.RemoveAllListeners();
            newAssignmentButton.onClick.AddListener(ShowEditor);
        }
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(Hide);
        }
        if (saveAssignmentButton != null)
        {
            saveAssignmentButton.onClick.RemoveAllListeners();
            saveAssignmentButton.onClick.AddListener(OnSave);
        }
        if (cancelEditorButton != null)
        {
            cancelEditorButton.onClick.RemoveAllListeners();
            cancelEditorButton.onClick.AddListener(HideEditor);
        }

        LoadAssignments();
    }

    public void Hide() { gameObject.SetActive(false); }

    private async void LoadAssignments()
    {
        assignments = await AssignmentService.GetGroupAssignments(currentGroupId);
        DisplayAssignments();
    }

    private void DisplayAssignments()
    {
        if (assignmentListContent == null) return;
        foreach (Transform child in assignmentListContent) Destroy(child.gameObject);

        if (assignments.Count == 0)
        {
            var emptyGO = new GameObject("Empty");
            emptyGO.transform.SetParent(assignmentListContent, false);
            var t = emptyGO.AddComponent<TextMeshProUGUI>();
            t.text = "Нет заданий. Нажмите «Создать»";
            t.fontSize = 22;
            t.alignment = TextAlignmentOptions.Center;
            emptyGO.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 50);
            return;
        }

        foreach (var a in assignments)
        {
            var row = new GameObject("Assignment_" + a.assignmentId);
            row.transform.SetParent(assignmentListContent, false);
            row.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 55);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(12, 12, 6, 6);
            layout.childForceExpandWidth = false;
            layout.childControlHeight = true;
            layout.childControlWidth = false;
            row.AddComponent<Image>().color = new Color(0.92f, 0.93f, 0.95f);
            row.AddComponent<LayoutElement>().preferredHeight = 55;

            var nameGO = new GameObject("Name");
            nameGO.transform.SetParent(row.transform, false);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = $"{a.name} ({a.questions.Count} вопр.)";
            nameTMP.fontSize = 20;
            nameGO.AddComponent<LayoutElement>().preferredWidth = 350;

            var statusGO = new GameObject("Status");
            statusGO.transform.SetParent(row.transform, false);
            var statusTMP = statusGO.AddComponent<TextMeshProUGUI>();
            statusTMP.text = a.active ? "Активно" : "Отключено";
            statusTMP.fontSize = 18;
            statusTMP.color = a.active ? new Color(0.2f, 0.7f, 0.3f) : Color.gray;
            statusGO.AddComponent<LayoutElement>().preferredWidth = 120;

            string aid = a.assignmentId;
            bool isActive = a.active;
            var toggleBtnGO = CreateSimpleButton(row.transform, isActive ? "Отключить" : "Включить",
                isActive ? new Color(0.85f, 0.55f, 0.15f) : new Color(0.2f, 0.65f, 0.35f), 110, 40);
            toggleBtnGO.GetComponent<Button>().onClick.AddListener(() => OnToggleAssignment(aid, !isActive));
        }
    }

    private async void OnToggleAssignment(string assignmentId, bool newState)
    {
        await AssignmentService.ToggleAssignment(assignmentId, newState);
        LoadAssignments();
    }

    private void ShowEditor()
    {
        if (assignmentListPanel != null) assignmentListPanel.SetActive(false);
        if (editorPanel != null) editorPanel.SetActive(true);
        if (assignmentNameInput != null) assignmentNameInput.text = "";
        if (editorStatusText != null) editorStatusText.text = "";
        ClearQuestionRows();
        WireAddButtons();
        AddQuestionRow(QuestionData.TypeMultipleChoice);
    }

    private void WireAddButtons()
    {
        if (addQuestionButton == null) return;
        addQuestionButton.onClick.RemoveAllListeners();
        addQuestionButton.onClick.AddListener(() => AddQuestionRow(QuestionData.TypeMultipleChoice));

        var parent = addQuestionButton.transform.parent;
        if (parent == null) return;
        var existingTF = parent.Find("AddTFBtn");
        if (existingTF != null) Destroy(existingTF.gameObject);
        var existingOT = parent.Find("AddOpenBtn");
        if (existingOT != null) Destroy(existingOT.gameObject);

        var tfBtn = CreateSimpleButton(parent, "+ Да/Нет", new Color(0.2f, 0.5f, 0.7f), 130, 35);
        tfBtn.name = "AddTFBtn";
        tfBtn.GetComponent<Button>().onClick.AddListener(() => AddQuestionRow(QuestionData.TypeTrueFalse));

        var otBtn = CreateSimpleButton(parent, "+ Текст", new Color(0.55f, 0.4f, 0.7f), 130, 35);
        otBtn.name = "AddOpenBtn";
        otBtn.GetComponent<Button>().onClick.AddListener(() => AddQuestionRow(QuestionData.TypeOpenText));
    }

    private void HideEditor()
    {
        if (editorPanel != null) editorPanel.SetActive(false);
        if (assignmentListPanel != null) assignmentListPanel.SetActive(true);
        ClearQuestionRows();
    }

    private void ClearQuestionRows()
    {
        foreach (var e in questionEntries)
            if (e.go != null) Destroy(e.go);
        questionEntries.Clear();
    }

    private void AddQuestionRow(int type)
    {
        if (questionsContent == null) return;
        int idx = questionEntries.Count;

        var go = new GameObject($"Question_{idx}");
        go.transform.SetParent(questionsContent, false);
        var rt = go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.94f, 0.94f, 0.96f);
        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(12, 12, 8, 8);
        vlg.spacing = 6;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var entry = new QuestionEntry { go = go, questionType = type };

        string typeLabel = type == QuestionData.TypeTrueFalse ? "Да/Нет"
            : type == QuestionData.TypeOpenText ? "Свободный ответ" : "Тест (4 варианта)";
        CreateLabel(go.transform, $"Вопрос {idx + 1} [{typeLabel}]:", 20, new Color(0.3f, 0.3f, 0.35f), 28);
        entry.textInput = CreateInput(go.transform, "Текст вопроса...", 40);

        if (type == QuestionData.TypeMultipleChoice)
        {
            rt.sizeDelta = new Vector2(0, 280);
            go.AddComponent<LayoutElement>().preferredHeight = 280;
            var optRow1 = CreateHRow(go.transform, 36);
            entry.optAInput = CreateInput(optRow1, "А)", 36);
            entry.optBInput = CreateInput(optRow1, "Б)", 36);
            var optRow2 = CreateHRow(go.transform, 36);
            entry.optCInput = CreateInput(optRow2, "В)", 36);
            entry.optDInput = CreateInput(optRow2, "Г)", 36);
            var correctRow = CreateHRow(go.transform, 32);
            CreateLabel(correctRow, "Правильный:", 18, Color.gray, 32);
            entry.correctDropdown = CreateDropdown(correctRow, new[] { "А", "Б", "В", "Г" }, 32);
        }
        else if (type == QuestionData.TypeTrueFalse)
        {
            rt.sizeDelta = new Vector2(0, 150);
            go.AddComponent<LayoutElement>().preferredHeight = 150;
            var correctRow = CreateHRow(go.transform, 32);
            CreateLabel(correctRow, "Правильный ответ:", 18, Color.gray, 32);
            entry.correctDropdown = CreateDropdown(correctRow, new[] { "Да", "Нет" }, 32);
        }
        else
        {
            rt.sizeDelta = new Vector2(0, 175);
            go.AddComponent<LayoutElement>().preferredHeight = 175;
            CreateLabel(go.transform, "Ожидаемый ответ:", 18, Color.gray, 24);
            entry.correctTextInput = CreateInput(go.transform, "Правильный ответ...", 38);
        }

        var removeGO = CreateSimpleButton(go.transform, "Удалить вопрос", new Color(0.8f, 0.25f, 0.25f), 0, 30);
        removeGO.GetComponent<LayoutElement>().flexibleWidth = 1;
        int capturedIdx = idx;
        removeGO.GetComponent<Button>().onClick.AddListener(() => RemoveQuestion(capturedIdx));

        questionEntries.Add(entry);
    }

    private void RemoveQuestion(int index)
    {
        if (index < 0 || index >= questionEntries.Count) return;
        Destroy(questionEntries[index].go);
        questionEntries.RemoveAt(index);
    }

    private async void OnSave()
    {
        string name = assignmentNameInput?.text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            if (editorStatusText != null) editorStatusText.text = "Введите название задания";
            return;
        }

        var questions = new List<QuestionData>();
        foreach (var e in questionEntries)
        {
            if (e.go == null) continue;
            string qText = e.textInput?.text?.Trim();
            if (string.IsNullOrEmpty(qText)) continue;

            var q = new QuestionData { questionType = e.questionType, text = qText };

            if (e.questionType == QuestionData.TypeMultipleChoice)
            {
                q.optionA = e.optAInput?.text?.Trim() ?? "";
                q.optionB = e.optBInput?.text?.Trim() ?? "";
                q.optionC = e.optCInput?.text?.Trim() ?? "";
                q.optionD = e.optDInput?.text?.Trim() ?? "";
                q.correctIndex = e.correctDropdown?.value ?? 0;
            }
            else if (e.questionType == QuestionData.TypeTrueFalse)
            {
                q.optionA = "Да";
                q.optionB = "Нет";
                q.correctIndex = e.correctDropdown?.value ?? 0;
            }
            else
            {
                q.correctTextAnswer = e.correctTextInput?.text?.Trim() ?? "";
            }

            questions.Add(q);
        }

        if (questions.Count == 0)
        {
            if (editorStatusText != null) editorStatusText.text = "Добавьте хотя бы один вопрос";
            return;
        }

        if (editorStatusText != null) editorStatusText.text = "Сохранение...";

        var result = await AssignmentService.CreateAssignment(name, currentGroupId, questions);
        if (result != null)
        {
            HideEditor();
            LoadAssignments();
        }
        else
        {
            if (editorStatusText != null) editorStatusText.text = "Ошибка сохранения";
        }
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string text, int size, Color color, float h)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        go.AddComponent<LayoutElement>().preferredHeight = h;
        return tmp;
    }

    private TMP_InputField CreateInput(Transform parent, string placeholder, float h)
    {
        var go = new GameObject("Input");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.96f, 0.96f, 0.98f);
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = h;
        le.flexibleWidth = 1;

        var ta = new GameObject("TextArea");
        ta.transform.SetParent(go.transform, false);
        var taRT = ta.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(8, 2);
        taRT.offsetMax = new Vector2(-8, -2);
        ta.AddComponent<RectMask2D>();

        var phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(ta.transform, false);
        var phRT = phGO.AddComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero; phRT.anchorMax = Vector2.one;
        phRT.offsetMin = phRT.offsetMax = Vector2.zero;
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder;
        phTMP.fontSize = 18;
        phTMP.fontStyle = FontStyles.Italic;
        phTMP.color = new Color(0.55f, 0.55f, 0.6f);

        var txGO = new GameObject("Text");
        txGO.transform.SetParent(ta.transform, false);
        var txRT = txGO.AddComponent<RectTransform>();
        txRT.anchorMin = Vector2.zero; txRT.anchorMax = Vector2.one;
        txRT.offsetMin = txRT.offsetMax = Vector2.zero;
        var txTMP = txGO.AddComponent<TextMeshProUGUI>();
        txTMP.fontSize = 18;
        txTMP.color = Color.black;

        var input = go.AddComponent<TMP_InputField>();
        input.textViewport = taRT;
        input.textComponent = txTMP;
        input.placeholder = phTMP;
        return input;
    }

    private Transform CreateHRow(Transform parent, float h)
    {
        var go = new GameObject("Row");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var layout = go.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childForceExpandWidth = true;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        go.AddComponent<LayoutElement>().preferredHeight = h;
        return go.transform;
    }

    private TMP_Dropdown CreateDropdown(Transform parent, string[] options, float h)
    {
        var go = new GameObject("Dropdown");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = Color.white;
        var dd = go.AddComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(new List<string>(options));
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 100;
        le.preferredHeight = h;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var lRT = labelGO.AddComponent<RectTransform>();
        lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one;
        lRT.offsetMin = new Vector2(8, 0); lRT.offsetMax = new Vector2(-25, 0);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.fontSize = 18;
        labelTMP.color = Color.black;
        dd.captionText = labelTMP;

        var templateGO = new GameObject("Template");
        templateGO.transform.SetParent(go.transform, false);
        templateGO.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 120);
        templateGO.AddComponent<Image>();
        var sr = templateGO.AddComponent<ScrollRect>();
        templateGO.SetActive(false);

        var vpGO = new GameObject("Viewport");
        vpGO.transform.SetParent(templateGO.transform, false);
        var vpRT = vpGO.AddComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        vpGO.AddComponent<Image>();
        vpGO.AddComponent<Mask>().showMaskGraphic = false;

        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(vpGO.transform, false);
        contentGO.AddComponent<RectTransform>();

        var itemGO = new GameObject("Item");
        itemGO.transform.SetParent(contentGO.transform, false);
        itemGO.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 32);
        itemGO.AddComponent<Toggle>();

        var itemLabelGO = new GameObject("Item Label");
        itemLabelGO.transform.SetParent(itemGO.transform, false);
        var ilRT = itemLabelGO.AddComponent<RectTransform>();
        ilRT.anchorMin = Vector2.zero; ilRT.anchorMax = Vector2.one;
        ilRT.offsetMin = new Vector2(8, 0); ilRT.offsetMax = Vector2.zero;
        var itemTMP = itemLabelGO.AddComponent<TextMeshProUGUI>();
        itemTMP.fontSize = 18;
        itemTMP.color = Color.black;

        dd.itemText = itemTMP;
        dd.template = templateGO.GetComponent<RectTransform>();
        sr.viewport = vpRT;
        sr.content = contentGO.GetComponent<RectTransform>();

        return dd;
    }

    private GameObject CreateSimpleButton(Transform parent, string text, Color bgColor, float w, float h)
    {
        var go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = bgColor;
        go.AddComponent<Button>();
        var le = go.AddComponent<LayoutElement>();
        if (w > 0) le.preferredWidth = w;
        le.preferredHeight = h;

        var txtGO = new GameObject("T");
        txtGO.transform.SetParent(go.transform, false);
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 16;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        var trt = txtGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        return go;
    }
}
