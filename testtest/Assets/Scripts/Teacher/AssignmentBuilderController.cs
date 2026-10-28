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
        public TMP_InputField textInput;
        public TMP_InputField optAInput;
        public TMP_InputField optBInput;
        public TMP_InputField optCInput;
        public TMP_InputField optDInput;
        public TMP_Dropdown correctDropdown;
    }

    public void Show(string groupId, string groupName)
    {
        Debug.Log($"[AssignmentBuilder] Show called for group={groupName} id={groupId}");
        currentGroupId = groupId;
        currentGroupName = groupName;
        gameObject.SetActive(true);

        if (assignmentListPanel != null) assignmentListPanel.SetActive(true);
        else Debug.LogWarning("[AssignmentBuilder] assignmentListPanel is NULL");
        if (editorPanel != null) editorPanel.SetActive(false);
        else Debug.LogWarning("[AssignmentBuilder] editorPanel is NULL");

        if (assignmentListTitle != null)
            assignmentListTitle.text = $"Задания: {groupName}";

        if (newAssignmentButton != null)
        {
            newAssignmentButton.onClick.RemoveAllListeners();
            newAssignmentButton.onClick.AddListener(ShowEditor);
            Debug.Log("[AssignmentBuilder] newAssignmentButton wired");
        }
        else Debug.LogWarning("[AssignmentBuilder] newAssignmentButton is NULL");

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(Hide);
        }
        if (addQuestionButton != null)
        {
            addQuestionButton.onClick.RemoveAllListeners();
            addQuestionButton.onClick.AddListener(AddQuestionRow);
        }
        else Debug.LogWarning("[AssignmentBuilder] addQuestionButton is NULL");
        if (saveAssignmentButton != null)
        {
            saveAssignmentButton.onClick.RemoveAllListeners();
            saveAssignmentButton.onClick.AddListener(OnSave);
        }
        else Debug.LogWarning("[AssignmentBuilder] saveAssignmentButton is NULL");
        if (cancelEditorButton != null)
        {
            cancelEditorButton.onClick.RemoveAllListeners();
            cancelEditorButton.onClick.AddListener(HideEditor);
        }

        LoadAssignments();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private async void LoadAssignments()
    {
        assignments = await AssignmentService.GetGroupAssignments(currentGroupId);
        DisplayAssignments();
    }

    private void DisplayAssignments()
    {
        if (assignmentListContent == null) return;

        foreach (Transform child in assignmentListContent)
            Destroy(child.gameObject);

        if (assignments.Count == 0)
        {
            var emptyGO = new GameObject("Empty");
            emptyGO.transform.SetParent(assignmentListContent, false);
            var t = emptyGO.AddComponent<TextMeshProUGUI>();
            t.text = "Нет заданий. Нажмите «Создать»";
            t.fontSize = 16;
            t.alignment = TextAlignmentOptions.Center;
            emptyGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 40);
            return;
        }

        foreach (var a in assignments)
        {
            var row = new GameObject("Assignment_" + a.assignmentId);
            row.transform.SetParent(assignmentListContent, false);
            var rowRT = row.AddComponent<RectTransform>();
            rowRT.sizeDelta = new Vector2(700, 50);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.childForceExpandWidth = false;
            row.AddComponent<Image>().color = new Color(0.92f, 0.93f, 0.95f);

            var nameGO = new GameObject("Name");
            nameGO.transform.SetParent(row.transform, false);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = $"{a.name} ({a.questions.Count} вопр.)";
            nameTMP.fontSize = 15;
            var nameLE = nameGO.AddComponent<LayoutElement>();
            nameLE.preferredWidth = 350;

            var statusGO = new GameObject("Status");
            statusGO.transform.SetParent(row.transform, false);
            var statusTMP = statusGO.AddComponent<TextMeshProUGUI>();
            statusTMP.text = a.active ? "✅ Активно" : "⛔ Отключено";
            statusTMP.fontSize = 14;
            statusTMP.color = a.active ? new Color(0.2f, 0.7f, 0.3f) : Color.gray;
            var statusLE = statusGO.AddComponent<LayoutElement>();
            statusLE.preferredWidth = 120;

            string aid = a.assignmentId;
            bool isActive = a.active;

            var toggleBtnGO = new GameObject("ToggleBtn");
            toggleBtnGO.transform.SetParent(row.transform, false);
            toggleBtnGO.AddComponent<Image>().color = isActive ? new Color(0.85f, 0.55f, 0.15f) : new Color(0.2f, 0.65f, 0.35f);
            var toggleBtn = toggleBtnGO.AddComponent<Button>();
            var toggleLE = toggleBtnGO.AddComponent<LayoutElement>();
            toggleLE.preferredWidth = 100;
            toggleLE.preferredHeight = 35;

            var toggleTxtGO = new GameObject("Text");
            toggleTxtGO.transform.SetParent(toggleBtnGO.transform, false);
            var toggleTxt = toggleTxtGO.AddComponent<TextMeshProUGUI>();
            toggleTxt.text = isActive ? "Отключить" : "Включить";
            toggleTxt.fontSize = 13;
            toggleTxt.color = Color.white;
            toggleTxt.alignment = TextAlignmentOptions.Center;
            var toggleTxtRT = toggleTxtGO.GetComponent<RectTransform>();
            toggleTxtRT.anchorMin = Vector2.zero;
            toggleTxtRT.anchorMax = Vector2.one;
            toggleTxtRT.offsetMin = toggleTxtRT.offsetMax = Vector2.zero;

            toggleBtn.onClick.AddListener(() => OnToggleAssignment(aid, !isActive));
        }
    }

    private async void OnToggleAssignment(string assignmentId, bool newState)
    {
        await AssignmentService.ToggleAssignment(assignmentId, newState);
        LoadAssignments();
    }

    private void ShowEditor()
    {
        Debug.Log("[AssignmentBuilder] ShowEditor called");
        if (assignmentListPanel != null) assignmentListPanel.SetActive(false);
        if (editorPanel != null)
        {
            editorPanel.SetActive(true);
            Debug.Log("[AssignmentBuilder] editorPanel activated");
        }
        if (assignmentNameInput != null) assignmentNameInput.text = "";
        if (editorStatusText != null) editorStatusText.text = "";
        ClearQuestionRows();
        AddQuestionRow();
        Debug.Log($"[AssignmentBuilder] Editor shown, questions={questionEntries.Count}, questionsContent={(questionsContent != null ? "SET" : "NULL")}");
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

    private void AddQuestionRow()
    {
        if (questionsContent == null)
        {
            Debug.LogError("[AssignmentBuilder] questionsContent is NULL, cannot add question row!");
            return;
        }
        int idx = questionEntries.Count;
        Debug.Log($"[AssignmentBuilder] AddQuestionRow idx={idx}");

        var go = new GameObject($"Question_{idx}");
        go.transform.SetParent(questionsContent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(650, 220);
        go.AddComponent<Image>().color = new Color(0.94f, 0.94f, 0.96f);
        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(10, 10, 8, 8);
        vlg.spacing = 4;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var entry = new QuestionEntry { go = go };

        var header = CreateLabel(go.transform, $"Вопрос {idx + 1}:", 15, Color.gray, 20);
        entry.textInput = CreateInput(go.transform, "Текст вопроса...", 35);

        var optRow1 = CreateHRow(go.transform, 30);
        entry.optAInput = CreateInput(optRow1, "А)", 30);
        entry.optBInput = CreateInput(optRow1, "Б)", 30);

        var optRow2 = CreateHRow(go.transform, 30);
        entry.optCInput = CreateInput(optRow2, "В)", 30);
        entry.optDInput = CreateInput(optRow2, "Г)", 30);

        var correctRow = CreateHRow(go.transform, 28);
        CreateLabel(correctRow, "Правильный:", 13, Color.gray, 28);
        entry.correctDropdown = CreateDropdown(correctRow, new[] { "А", "Б", "В", "Г" }, 28);

        var removeGO = new GameObject("RemoveBtn");
        removeGO.transform.SetParent(go.transform, false);
        removeGO.AddComponent<Image>().color = new Color(0.8f, 0.25f, 0.25f);
        var removeBtn = removeGO.AddComponent<Button>();
        var removeLe = removeGO.AddComponent<LayoutElement>();
        removeLe.preferredHeight = 25;
        var removeTxt = new GameObject("T").AddComponent<TextMeshProUGUI>();
        removeTxt.transform.SetParent(removeGO.transform, false);
        removeTxt.text = "Удалить вопрос";
        removeTxt.fontSize = 12;
        removeTxt.color = Color.white;
        removeTxt.alignment = TextAlignmentOptions.Center;
        var removeTxtRT = removeTxt.GetComponent<RectTransform>();
        removeTxtRT.anchorMin = Vector2.zero;
        removeTxtRT.anchorMax = Vector2.one;
        removeTxtRT.offsetMin = removeTxtRT.offsetMax = Vector2.zero;

        int capturedIdx = idx;
        removeBtn.onClick.AddListener(() => RemoveQuestion(capturedIdx));

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
            questions.Add(new QuestionData
            {
                text = qText,
                optionA = e.optAInput?.text?.Trim() ?? "",
                optionB = e.optBInput?.text?.Trim() ?? "",
                optionC = e.optCInput?.text?.Trim() ?? "",
                optionD = e.optDInput?.text?.Trim() ?? "",
                correctIndex = e.correctDropdown?.value ?? 0
            });
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

    // --- UI helpers ---

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
        taRT.offsetMin = new Vector2(6, 2);
        taRT.offsetMax = new Vector2(-6, -2);
        ta.AddComponent<RectMask2D>();

        var phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(ta.transform, false);
        var phRT = phGO.AddComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero; phRT.anchorMax = Vector2.one;
        phRT.offsetMin = phRT.offsetMax = Vector2.zero;
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder;
        phTMP.fontSize = 14;
        phTMP.fontStyle = FontStyles.Italic;
        phTMP.color = new Color(0.6f, 0.6f, 0.65f);

        var txGO = new GameObject("Text");
        txGO.transform.SetParent(ta.transform, false);
        var txRT = txGO.AddComponent<RectTransform>();
        txRT.anchorMin = Vector2.zero; txRT.anchorMax = Vector2.one;
        txRT.offsetMin = txRT.offsetMax = Vector2.zero;
        var txTMP = txGO.AddComponent<TextMeshProUGUI>();
        txTMP.fontSize = 14;
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
        layout.spacing = 6;
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
        le.preferredWidth = 80;
        le.preferredHeight = h;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var lRT = labelGO.AddComponent<RectTransform>();
        lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one;
        lRT.offsetMin = new Vector2(8, 0); lRT.offsetMax = new Vector2(-25, 0);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.fontSize = 14;
        labelTMP.color = Color.black;
        dd.captionText = labelTMP;

        var templateGO = new GameObject("Template");
        templateGO.transform.SetParent(go.transform, false);
        templateGO.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
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
        itemGO.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 28);
        var itemToggle = itemGO.AddComponent<Toggle>();

        var itemLabelGO = new GameObject("Item Label");
        itemLabelGO.transform.SetParent(itemGO.transform, false);
        var ilRT = itemLabelGO.AddComponent<RectTransform>();
        ilRT.anchorMin = Vector2.zero; ilRT.anchorMax = Vector2.one;
        ilRT.offsetMin = new Vector2(8, 0); ilRT.offsetMax = Vector2.zero;
        var itemTMP = itemLabelGO.AddComponent<TextMeshProUGUI>();
        itemTMP.fontSize = 14;
        itemTMP.color = Color.black;

        dd.itemText = itemTMP;
        dd.template = templateGO.GetComponent<RectTransform>();
        sr.viewport = vpRT;
        sr.content = contentGO.GetComponent<RectTransform>();

        return dd;
    }
}
