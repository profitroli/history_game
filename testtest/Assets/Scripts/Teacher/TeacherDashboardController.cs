using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TeacherDashboardController : MonoBehaviour
{
    [Header("Основной экран")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Button createGroupButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button previewAsStudentButton;
    [SerializeField] private Transform groupsListContent;
    [SerializeField] private GameObject groupCardPrefab;

    [Header("Панель создания группы")]
    [SerializeField] private GameObject createGroupPanel;
    [SerializeField] private TMP_InputField groupNameInput;
    [SerializeField] private Toggle schoolProgramToggle;
    [SerializeField] private Button confirmCreateButton;
    [SerializeField] private Button cancelCreateButton;
    [SerializeField] private TMP_Text createdCodeText;
    [SerializeField] private GameObject codeDisplayPanel;

    [Header("Панель просмотра группы")]
    [SerializeField] private GameObject groupDetailPanel;
    [SerializeField] private TMP_Text groupDetailName;
    [SerializeField] private TMP_Text groupDetailCode;
    [SerializeField] private TMP_Text groupDetailProgram;
    [SerializeField] private Transform studentsListContent;
    [SerializeField] private GameObject studentRowPrefab;
    [SerializeField] private Button backFromDetailButton;
    [SerializeField] private Button refreshButton;

    private List<GroupData> groups = new List<GroupData>();
    private string selectedGroupId;

    void Start()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsTeacher)
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        if (headerText != null)
            headerText.text = $"Кабинет преподавателя — {SessionManager.Instance.DisplayName}";

        if (createGroupButton != null) createGroupButton.onClick.AddListener(ShowCreateGroupPanel);
        if (logoutButton != null) logoutButton.onClick.AddListener(Logout);
        if (previewAsStudentButton != null) previewAsStudentButton.onClick.AddListener(PreviewAsStudent);
        if (confirmCreateButton != null) confirmCreateButton.onClick.AddListener(OnCreateGroup);
        if (cancelCreateButton != null) cancelCreateButton.onClick.AddListener(HideCreateGroupPanel);
        if (backFromDetailButton != null) backFromDetailButton.onClick.AddListener(ShowGroupsList);
        if (refreshButton != null) refreshButton.onClick.AddListener(RefreshStudents);

        HideAllPanels();
        LoadGroups();
    }

    private async void LoadGroups()
    {
        groups = await GroupService.GetTeacherGroups();
        DisplayGroups();
    }

    private void DisplayGroups()
    {
        if (groupsListContent == null) return;

        foreach (Transform child in groupsListContent)
            Destroy(child.gameObject);

        if (groups.Count == 0)
        {
            var emptyObj = new GameObject("EmptyMessage");
            emptyObj.transform.SetParent(groupsListContent, false);
            var text = emptyObj.AddComponent<TextMeshProUGUI>();
            text.text = "Нет созданных групп. Нажмите «Создать группу»";
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            var rt = emptyObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600, 60);
            return;
        }

        foreach (var group in groups)
        {
            GameObject card;
            if (groupCardPrefab != null)
            {
                card = Instantiate(groupCardPrefab, groupsListContent);
            }
            else
            {
                card = CreateDefaultGroupCard(group);
                card.transform.SetParent(groupsListContent, false);
            }

            var texts = card.GetComponentsInChildren<TMP_Text>();
            if (texts.Length > 0) texts[0].text = group.name;
            if (texts.Length > 1) texts[1].text = $"Код: {group.code} | {(group.IsSchool ? "Школьная" : "Студенческая")}";

            string gId = group.groupId;
            var btn = card.GetComponentInChildren<Button>();
            if (btn != null) btn.onClick.AddListener(() => ShowGroupDetail(gId));
        }
    }

    private GameObject CreateDefaultGroupCard(GroupData group)
    {
        var card = new GameObject("GroupCard");
        var rt = card.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 80);
        var layout = card.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 5, 5);
        layout.spacing = 10;
        var bg = card.AddComponent<Image>();
        bg.color = new Color(0.9f, 0.9f, 0.95f, 1f);

        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(card.transform, false);
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = $"{group.name}\nКод: {group.code} | {(group.IsSchool ? "Школьная" : "Студенческая")}";
        nameText.fontSize = 16;

        var btnObj = new GameObject("OpenBtn");
        btnObj.transform.SetParent(card.transform, false);
        var btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.5f, 0.8f, 1f);
        var btnComp = btnObj.AddComponent<Button>();
        var btnRt = btnObj.GetComponent<RectTransform>();
        btnRt.sizeDelta = new Vector2(100, 40);
        var btnText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        btnText.transform.SetParent(btnObj.transform, false);
        btnText.text = "Открыть";
        btnText.fontSize = 14;
        btnText.alignment = TextAlignmentOptions.Center;

        return card;
    }

    private async void ShowGroupDetail(string groupId)
    {
        selectedGroupId = groupId;
        var group = groups.Find(g => g.groupId == groupId);
        if (group == null) return;

        if (groupDetailPanel != null) groupDetailPanel.SetActive(true);
        if (groupDetailName != null) groupDetailName.text = group.name;
        if (groupDetailCode != null) groupDetailCode.text = $"Код: {group.code}";
        if (groupDetailProgram != null) groupDetailProgram.text = group.IsSchool ? "Школьная программа" : "Студенческая программа";

        await LoadStudents(groupId);
    }

    private async System.Threading.Tasks.Task LoadStudents(string groupId)
    {
        if (studentsListContent == null) return;

        foreach (Transform child in studentsListContent)
            Destroy(child.gameObject);

        var students = await GroupService.GetGroupStudents(groupId);

        if (students.Count == 0)
        {
            var emptyObj = new GameObject("NoStudents");
            emptyObj.transform.SetParent(studentsListContent, false);
            var text = emptyObj.AddComponent<TextMeshProUGUI>();
            text.text = "Пока нет учеников в этой группе";
            text.fontSize = 16;
            var rt = emptyObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600, 40);
            return;
        }

        foreach (var student in students)
        {
            var progress = await GroupService.GetStudentProgress(student.uid);

            GameObject row;
            if (studentRowPrefab != null)
            {
                row = Instantiate(studentRowPrefab, studentsListContent);
                var rowComp = row.GetComponent<StudentProgressRow>();
                if (rowComp != null) rowComp.SetData(student, progress);
            }
            else
            {
                row = CreateDefaultStudentRow(student, progress);
                row.transform.SetParent(studentsListContent, false);
            }
        }
    }

    private GameObject CreateDefaultStudentRow(UserData student, ProgressData progress)
    {
        var row = new GameObject("StudentRow");
        var rt = row.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(700, 40);
        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15;
        layout.padding = new RectOffset(10, 10, 5, 5);
        var bg = row.AddComponent<Image>();
        bg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        AddTextToRow(row, student.displayName, 200);
        AddTextToRow(row, $"Лекции: {progress.LecturesCompleted}", 120);
        AddTextToRow(row, $"Игры: {progress.GamesCompleted}", 120);
        AddTextToRow(row, $"Тесты: {progress.TestsCompleted}", 120);
        AddTextToRow(row, $"Ср. балл: {progress.AverageTestScore:F0}%", 120);

        return row;
    }

    private void AddTextToRow(GameObject parent, string value, float width)
    {
        var obj = new GameObject("Col");
        obj.transform.SetParent(parent.transform, false);
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = 14;
        var rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 30);
        var le = obj.AddComponent<LayoutElement>();
        le.preferredWidth = width;
    }

    private void ShowCreateGroupPanel()
    {
        if (createGroupPanel != null) createGroupPanel.SetActive(true);
        if (codeDisplayPanel != null) codeDisplayPanel.SetActive(false);
        if (groupNameInput != null) groupNameInput.text = "";
    }

    private void HideCreateGroupPanel()
    {
        if (createGroupPanel != null) createGroupPanel.SetActive(false);
    }

    private async void OnCreateGroup()
    {
        string name = groupNameInput?.text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Введите название группы");
            return;
        }

        string program = (schoolProgramToggle != null && schoolProgramToggle.isOn) ? "school" : "university";
        var group = await GroupService.CreateGroup(name, program);

        if (group != null)
        {
            if (createdCodeText != null) createdCodeText.text = $"Код группы: {group.code}";
            if (codeDisplayPanel != null) codeDisplayPanel.SetActive(true);
            LoadGroups();
        }
    }

    private void ShowGroupsList()
    {
        if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
    }

    private void RefreshStudents()
    {
        if (!string.IsNullOrEmpty(selectedGroupId))
            _ = LoadStudents(selectedGroupId);
    }

    private void HideAllPanels()
    {
        if (createGroupPanel != null) createGroupPanel.SetActive(false);
        if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
        if (codeDisplayPanel != null) codeDisplayPanel.SetActive(false);
    }

    private void PreviewAsStudent()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Logout()
    {
        FirebaseManager.Instance?.Service?.SignOut();
        SessionManager.Instance?.Clear();
        SceneManager.LoadScene("LoginScene");
    }
}
