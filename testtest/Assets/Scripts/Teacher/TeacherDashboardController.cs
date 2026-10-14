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
    [SerializeField] private Button exportCsvButton;
    [SerializeField] private Button archiveGroupButton;
    [SerializeField] private Toggle showArchivedToggle;

    [Header("Создание группы — доп. поля")]
    [SerializeField] private TMP_InputField institutionInput;

    private List<GroupData> groups = new List<GroupData>();
    private List<UserData> currentStudents = new List<UserData>();
    private List<ProgressData> currentProgressList = new List<ProgressData>();
    private string selectedGroupId;
    private bool showArchived;

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
        if (exportCsvButton != null) exportCsvButton.onClick.AddListener(ExportToCsv);
        if (archiveGroupButton != null) archiveGroupButton.onClick.AddListener(ArchiveSelectedGroup);
        if (showArchivedToggle != null) showArchivedToggle.onValueChanged.AddListener(OnShowArchivedChanged);

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

        var visibleGroups = groups.FindAll(g => showArchived || !g.archived);
        if (visibleGroups.Count == 0)
        {
            var emptyObj2 = new GameObject("EmptyMsg");
            emptyObj2.transform.SetParent(groupsListContent, false);
            var t = emptyObj2.AddComponent<TextMeshProUGUI>();
            t.text = showArchived ? "Нет архивных групп" : "Нет активных групп. Нажмите «Создать группу»";
            t.fontSize = 18;
            t.alignment = TextAlignmentOptions.Center;
            var r = emptyObj2.GetComponent<RectTransform>();
            r.sizeDelta = new Vector2(600, 60);
            return;
        }

        foreach (var group in visibleGroups)
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
        currentStudents = students;
        currentProgressList.Clear();

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
            currentProgressList.Add(progress);

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
        string institution = institutionInput?.text?.Trim() ?? "";
        var group = await GroupService.CreateGroup(name, program, institution);

        if (group != null)
        {
            string inviteLink = GroupService.GetInviteLink(group.code);
            if (createdCodeText != null) createdCodeText.text = $"Код группы: {group.code}\nСсылка: {inviteLink}";
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

    private void OnShowArchivedChanged(bool show)
    {
        showArchived = show;
        DisplayGroups();
    }

    private async void ArchiveSelectedGroup()
    {
        if (string.IsNullOrEmpty(selectedGroupId)) return;

        var group = groups.Find(g => g.groupId == selectedGroupId);
        if (group == null) return;

        bool newState = !group.archived;
        var data = new Dictionary<string, object> { { "archived", newState } };
        bool ok = await FirebaseManager.Instance.Service.UpdateDocument("groups", selectedGroupId, data);

        if (ok)
        {
            group.archived = newState;
            Debug.Log($"[Teacher] Группа {group.name} {(newState ? "архивирована" : "восстановлена")}");
            if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
            DisplayGroups();
        }
    }

    private void ExportToCsv()
    {
        if (currentStudents.Count == 0)
        {
            Debug.LogWarning("[Teacher] Нет данных для экспорта");
            return;
        }

        var group = groups.Find(g => g.groupId == selectedGroupId);
        string groupName = group?.name ?? "group";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Имя,Лекции,Игры,Тесты,Средний балл %");

        for (int i = 0; i < currentStudents.Count; i++)
        {
            var student = currentStudents[i];
            var progress = i < currentProgressList.Count ? currentProgressList[i] : new ProgressData();
            sb.AppendLine($"\"{student.displayName}\",{progress.LecturesCompleted},{progress.GamesCompleted},{progress.TestsCompleted},{progress.AverageTestScore:F0}");
        }

        string csv = sb.ToString();

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadCsvWebGL(csv, $"{groupName}_report.csv");
#else
        string path = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
            $"{groupName}_report.csv");
        System.IO.File.WriteAllText(path, csv, System.Text.Encoding.UTF8);
        Debug.Log($"[Teacher] Отчёт сохранён: {path}");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void DownloadCsvWebGL(string csv, string filename);
#else
    private static void DownloadCsvWebGL(string csv, string filename) { }
#endif
}
