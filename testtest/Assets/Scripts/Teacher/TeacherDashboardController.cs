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

    [Header("Конструктор заданий")]
    [SerializeField] private Button assignmentsButton;
    [SerializeField] private AssignmentBuilderController assignmentBuilder;

    [Header("Сайдбар")]
    [SerializeField] private TMP_Text sidebarNameText;
    [SerializeField] private TMP_Text sidebarRoleText;
    [SerializeField] private Image sidebarAvatarImage;
    [SerializeField] private Button navHomeButton;
    [SerializeField] private Button navDataButton;
    [SerializeField] private Button navGroupsButton;
    [SerializeField] private Button navAssignmentsButton;
    [SerializeField] private Button navMyTasksButton;
    [SerializeField] private Button navGradingButton;
    [SerializeField] private Button navStatsButton;
    [SerializeField] private Button navSettingsButton;
    [SerializeField] private Button navExitButton;

    [Header("Подсветка навигации")]
    [SerializeField] private Image navHomeHighlight;
    [SerializeField] private Image navDataHighlight;
    [SerializeField] private Image navGroupsHighlight;
    [SerializeField] private Image navAssignmentsHighlight;
    [SerializeField] private Image navMyTasksHighlight;
    [SerializeField] private Image navGradingHighlight;
    [SerializeField] private Image navStatsHighlight;
    [SerializeField] private Image navSettingsHighlight;

    [Header("Виды контента")]
    [SerializeField] private GameObject homeView;

    private static readonly Color NavActive = new Color(0.30f, 0.50f, 0.22f, 1f);
    private static readonly Color NavInactive = new Color(0f, 0f, 0f, 0f);

    private List<GroupData> groups = new List<GroupData>();
    private List<UserData> currentStudents = new List<UserData>();
    private List<ProgressData> currentProgressList = new List<ProgressData>();
    private string selectedGroupId;
    private bool showArchived;
    private GameObject assignmentBuilderPanel;

    void Start()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsTeacher)
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        if (headerText != null)
            headerText.text = "ПАНЕЛЬ УЧИТЕЛЯ";

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

        WireSidebarFromScene();
        WireAssignmentBuilder();

        if (navHomeButton != null) navHomeButton.onClick.AddListener(ShowHome);
        if (navDataButton != null) navDataButton.onClick.AddListener(() => OnNavShowGroup(0));
        if (navGroupsButton != null) navGroupsButton.onClick.AddListener(ShowGroupsSelection);
        if (navAssignmentsButton != null) navAssignmentsButton.onClick.AddListener(ShowAssignments);
        if (navMyTasksButton != null) navMyTasksButton.onClick.AddListener(ShowAssignments);
        if (navGradingButton != null) navGradingButton.onClick.AddListener(() => OnNavShowGroup(0));
        if (navStatsButton != null) navStatsButton.onClick.AddListener(() => OnNavShowGroup(0));
        if (navSettingsButton != null) navSettingsButton.onClick.AddListener(ShowHome);
        if (navExitButton != null) navExitButton.onClick.AddListener(Logout);

        WireActionCards();
        WireInfoCardButtons();
        UpdateSidebar();
        ShowHome();
        LoadGroups();
    }

    private void WireAssignmentBuilder()
    {
        if (assignmentBuilder != null)
        {
            assignmentBuilderPanel = assignmentBuilder.gameObject;
            return;
        }
        assignmentBuilder = FindAnyObjectByType<AssignmentBuilderController>(FindObjectsInactive.Include);
        if (assignmentBuilder != null)
        {
            assignmentBuilderPanel = assignmentBuilder.gameObject;
            return;
        }
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        var ca = canvas.transform.Find("ContentArea");
        if (ca == null) return;
        var abp = ca.Find("AssignmentBuilderPanel");
        if (abp != null)
        {
            assignmentBuilder = abp.GetComponent<AssignmentBuilderController>();
            assignmentBuilderPanel = abp.gameObject;
        }
    }

    private void WireActionCards()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        var contentArea = canvas.transform.Find("ContentArea");
        var hv = contentArea != null ? contentArea.Find("HomeView") : null;
        var cardsRow = hv != null ? hv.Find("ActionCardsRow") : null;
        if (cardsRow == null) return;

        for (int i = 0; i < cardsRow.childCount; i++)
        {
            var card = cardsRow.GetChild(i);
            var btn = card.GetComponentInChildren<Button>();
            if (btn == null) continue;
            int idx = i;
            btn.onClick.AddListener(() => OnActionCardClicked(idx));
        }
    }

    private void WireInfoCardButtons()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        var ca = canvas.transform.Find("ContentArea");
        var hv = ca != null ? ca.Find("HomeView") : null;
        if (hv == null) return;

        var infoRow = hv.Find("InfoCardsRow");
        if (infoRow == null) infoRow = hv.Find("InfoRow");
        if (infoRow == null) return;

        var groupsBtn = FindActionBtn(infoRow, "GroupsCard");
        if (groupsBtn != null) groupsBtn.onClick.AddListener(ShowGroupsSelection);

        var assignBtn = FindActionBtn(infoRow, "AssignmentsCard");
        if (assignBtn != null) assignBtn.onClick.AddListener(ShowAssignments);

        var statsBtn = FindActionBtn(infoRow, "StatsCard");
        if (statsBtn != null) statsBtn.onClick.AddListener(() => OnNavShowGroup(0));
    }

    private Button FindActionBtn(Transform parent, string cardName)
    {
        var card = parent.Find(cardName);
        if (card == null) return null;
        var ab = card.Find("ActionBtn");
        return ab != null ? ab.GetComponent<Button>() : null;
    }

    private void ShowHome()
    {
        HideAllPanels();
        if (homeView != null) homeView.SetActive(true);
        HighlightNav(navHomeHighlight);
    }

    private void ShowGroupsSelection()
    {
        if (groups.Count == 0)
        {
            ShowCreateGroupPanel();
            return;
        }
        if (groups.Count == 1)
        {
            ShowGroupDetail(groups[0].groupId);
            return;
        }
        HideAllPanels();
        if (homeView != null) homeView.SetActive(true);
        HighlightNav(navGroupsHighlight);
        ToastManager.Show($"Групп: {groups.Count}. Нажмите «Открыть» на карточке.");
    }

    private void ShowAssignments()
    {
        if (assignmentBuilder == null) WireAssignmentBuilder();
        if (assignmentBuilder != null && groups.Count > 0)
        {
            HideAllPanels();
            string gId = !string.IsNullOrEmpty(selectedGroupId) ? selectedGroupId : groups[0].groupId;
            var g = groups.Find(x => x.groupId == gId);
            assignmentBuilder.Show(gId, g != null ? g.name : "Группа");
            HighlightNav(navAssignmentsHighlight);
        }
        else if (groups.Count == 0)
            ToastManager.Show("Сначала создайте группу");
        else
            ToastManager.Show("Конструктор заданий не найден");
    }

    private void OnNavShowGroup(int fallbackIndex)
    {
        if (groups.Count > 0)
        {
            string gId = !string.IsNullOrEmpty(selectedGroupId) ? selectedGroupId : groups[0].groupId;
            ShowGroupDetail(gId);
        }
        else
            ToastManager.Show("Сначала создайте группу");
    }

    private void HighlightNav(Image active)
    {
        Image[] all = { navHomeHighlight, navDataHighlight, navGroupsHighlight, navAssignmentsHighlight,
                        navMyTasksHighlight, navGradingHighlight, navStatsHighlight, navSettingsHighlight };
        foreach (var h in all)
            if (h != null) h.color = (h == active) ? NavActive : NavInactive;
    }

    private void UpdateSidebar()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        if (sidebarNameText != null) sidebarNameText.text = SessionManager.Instance.DisplayName;
        if (sidebarRoleText != null) sidebarRoleText.text = "Учитель";
    }

    private void WireSidebarFromScene()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        var sidebar = canvas.transform.Find("LeftSidebar");
        if (sidebar == null) return;

        if (sidebarNameText == null) { var go = sidebar.Find("SidebarName"); if (go != null) sidebarNameText = go.GetComponent<TMP_Text>(); }
        if (sidebarRoleText == null) { var go = sidebar.Find("SidebarRole"); if (go != null) sidebarRoleText = go.GetComponent<TMP_Text>(); }

        if (navHomeButton == null) WireNavButton(sidebar, "NavHome", ref navHomeButton, ref navHomeHighlight);
        if (navDataButton == null) WireNavButton(sidebar, "NavData", ref navDataButton, ref navDataHighlight);
        if (navGroupsButton == null) WireNavButton(sidebar, "NavGroups", ref navGroupsButton, ref navGroupsHighlight);
        if (navAssignmentsButton == null) WireNavButton(sidebar, "NavAssignments", ref navAssignmentsButton, ref navAssignmentsHighlight);
        if (navMyTasksButton == null) WireNavButton(sidebar, "NavMyTasks", ref navMyTasksButton, ref navMyTasksHighlight);
        if (navGradingButton == null) WireNavButton(sidebar, "NavGrading", ref navGradingButton, ref navGradingHighlight);
        if (navStatsButton == null) WireNavButton(sidebar, "NavStats", ref navStatsButton, ref navStatsHighlight);
        if (navSettingsButton == null) WireNavButton(sidebar, "NavSettings", ref navSettingsButton, ref navSettingsHighlight);
        if (navExitButton == null) { var go = sidebar.Find("NavExit"); if (go != null) navExitButton = go.GetComponent<Button>(); }

        if (homeView == null) { var ca = canvas.transform.Find("ContentArea"); if (ca != null) { var hv = ca.Find("HomeView"); if (hv != null) homeView = hv.gameObject; } }
    }

    private void WireNavButton(Transform sidebar, string name, ref Button btn, ref Image highlight)
    {
        var go = sidebar.Find(name);
        if (go == null) return;
        if (btn == null) btn = go.GetComponent<Button>();
        if (highlight == null) highlight = go.GetComponent<Image>();
    }

    private void OnActionCardClicked(int cardIndex)
    {
        switch (cardIndex)
        {
            case 0:
                if (groups.Count > 0)
                    ShowGroupDetail(groups[0].groupId);
                else
                    ToastManager.Show("Сначала создайте группу");
                break;
            case 1:
                ShowGroupsSelection();
                break;
            case 2:
                ShowAssignments();
                break;
            case 3:
                PreviewAsStudent();
                break;
        }
    }

    private async void LoadGroups()
    {
        groups = await GroupService.GetTeacherGroups();
        DisplayGroups();
        UpdateHomeInfoCards();
    }

    private void UpdateHomeInfoCards()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        var contentArea = canvas.transform.Find("ContentArea");
        var hv = contentArea != null ? contentArea.Find("HomeView") : null;
        if (hv == null) return;

        var infoRow = hv.Find("InfoCardsRow");
        if (infoRow == null) infoRow = hv.Find("InfoRow");
        if (infoRow == null) return;

        UpdateCardText(infoRow, "GroupsCard", groups.Count == 0
            ? "Нет групп.\nНажмите «Создать» чтобы начать"
            : BuildGroupsList());

        if (groups.Count > 0)
        {
            var aph = GetCardPlaceholder(infoRow, "AssignmentsCard");
            if (aph != null) _ = UpdateAssignmentsInfo(aph, groups[0].groupId);
            var sph = GetCardPlaceholder(infoRow, "StatsCard");
            if (sph != null) _ = UpdateStatsInfo(sph, groups[0].groupId);
        }
        else
        {
            UpdateCardText(infoRow, "AssignmentsCard", "Создайте группу,\nзатем добавьте задания");
            UpdateCardText(infoRow, "StatsCard", "Статистика появится\nпосле добавления учеников");
        }
    }

    private string BuildGroupsList()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var g in groups)
            sb.AppendLine($"• {g.name}  ({g.code})");
        return sb.ToString().TrimEnd();
    }

    private void UpdateCardText(Transform infoRow, string cardName, string text)
    {
        var t = GetCardPlaceholder(infoRow, cardName);
        if (t != null) t.text = text;
    }

    private TMP_Text GetCardPlaceholder(Transform infoRow, string cardName)
    {
        var card = infoRow.Find(cardName);
        if (card == null) return null;
        var ph = card.Find("Placeholder");
        return ph != null ? ph.GetComponent<TMP_Text>() : null;
    }

    private async System.Threading.Tasks.Task UpdateAssignmentsInfo(TMP_Text text, string groupId)
    {
        var assignments = await AssignmentService.GetGroupAssignments(groupId);
        if (assignments.Count == 0)
            text.text = "Заданий пока нет.\nНажмите «Создать» для нового";
        else
        {
            var sb = new System.Text.StringBuilder();
            foreach (var a in assignments)
                sb.AppendLine($"• {a.name} ({a.questions.Count} вопр.)");
            text.text = sb.ToString().TrimEnd();
        }
    }

    private async System.Threading.Tasks.Task UpdateStatsInfo(TMP_Text text, string groupId)
    {
        var students = await GroupService.GetGroupStudents(groupId);
        if (students.Count == 0) { text.text = "Учеников пока нет"; return; }
        float totalScore = 0; int count = 0;
        foreach (var s in students)
        {
            var p = await GroupService.GetStudentProgress(s.uid);
            if (p.TestsCompleted > 0) { totalScore += p.AverageTestScore; count++; }
        }
        float avg = count > 0 ? totalScore / count : 0;
        text.text = $"Учеников: {students.Count}\nСредний балл: {avg:F0}%";
    }

    private void DisplayGroups()
    {
        if (groupsListContent == null) return;
        foreach (Transform child in groupsListContent) Destroy(child.gameObject);

        if (groups.Count == 0)
        {
            var emptyObj = new GameObject("EmptyMessage");
            emptyObj.transform.SetParent(groupsListContent, false);
            var text = emptyObj.AddComponent<TextMeshProUGUI>();
            text.text = "Нет созданных групп. Нажмите «Создать группу»";
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            emptyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 60);
            return;
        }

        var visibleGroups = groups.FindAll(g => showArchived || !g.archived);
        foreach (var group in visibleGroups)
        {
            GameObject card = groupCardPrefab != null
                ? Instantiate(groupCardPrefab, groupsListContent)
                : CreateDefaultGroupCard(group);
            if (groupCardPrefab == null) card.transform.SetParent(groupsListContent, false);

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
        card.AddComponent<RectTransform>().sizeDelta = new Vector2(600, 80);
        var layout = card.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 5, 5);
        layout.spacing = 10;
        card.AddComponent<Image>().color = new Color(0.9f, 0.9f, 0.95f, 1f);

        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(card.transform, false);
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = $"{group.name}\nКод: {group.code} | {(group.IsSchool ? "Школьная" : "Студенческая")}";
        nameText.fontSize = 16;

        var btnObj = new GameObject("OpenBtn");
        btnObj.transform.SetParent(card.transform, false);
        btnObj.AddComponent<Image>().color = new Color(0.3f, 0.5f, 0.8f, 1f);
        btnObj.AddComponent<Button>();
        btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);
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

        HideAllPanels();
        if (groupDetailPanel != null) groupDetailPanel.SetActive(true);
        HighlightNav(navGroupsHighlight);
        if (groupDetailName != null) groupDetailName.text = group.name;
        if (groupDetailCode != null) groupDetailCode.text = $"Код: {group.code}";
        if (groupDetailProgram != null) groupDetailProgram.text = group.IsSchool ? "Школьная программа" : "Студенческая программа";

        if (assignmentsButton != null)
        {
            assignmentsButton.onClick.RemoveAllListeners();
            string gId = groupId;
            string gName = group.name;
            assignmentsButton.onClick.AddListener(() =>
            {
                if (assignmentBuilder != null) assignmentBuilder.Show(gId, gName);
            });
        }

        await LoadStudents(groupId);
    }

    private async System.Threading.Tasks.Task LoadStudents(string groupId)
    {
        if (studentsListContent == null) return;
        foreach (Transform child in studentsListContent) Destroy(child.gameObject);

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
            emptyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 40);
            return;
        }

        foreach (var student in students)
        {
            var progress = await GroupService.GetStudentProgress(student.uid);
            currentProgressList.Add(progress);
            GameObject row = studentRowPrefab != null
                ? Instantiate(studentRowPrefab, studentsListContent)
                : CreateDefaultStudentRow(student, progress);
            if (studentRowPrefab != null)
            {
                var rowComp = row.GetComponent<StudentProgressRow>();
                if (rowComp != null) rowComp.SetData(student, progress);
            }
            else row.transform.SetParent(studentsListContent, false);
        }
    }

    private GameObject CreateDefaultStudentRow(UserData student, ProgressData progress)
    {
        var row = new GameObject("StudentRow");
        row.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 50);
        var rowLE = row.AddComponent<LayoutElement>();
        rowLE.minHeight = 50;
        rowLE.preferredHeight = 50;
        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15;
        layout.padding = new RectOffset(10, 10, 5, 5);
        layout.childForceExpandHeight = true;
        layout.childControlHeight = true;
        layout.childControlWidth = false;
        row.AddComponent<Image>().color = new Color(0.92f, 0.90f, 0.85f, 1f);

        AddTextToRow(row, student.displayName, 200, true);
        AddTextToRow(row, $"Лекции: {progress.LecturesCompleted}/14", 130);
        AddTextToRow(row, $"Игры: {progress.GamesCompleted}/35", 120);
        AddTextToRow(row, $"Тесты: {progress.TestsCompleted}/14", 120);
        AddTextToRow(row, $"Ср. балл: {progress.AverageTestScore:F0}%", 140);

        return row;
    }

    private void AddTextToRow(GameObject parent, string value, float width, bool bold = false)
    {
        var obj = new GameObject("Col");
        obj.transform.SetParent(parent.transform, false);
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = 18;
        text.color = new Color(0.24f, 0.16f, 0.1f, 1f);
        if (bold) text.fontStyle = FontStyles.Bold;
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 40);
        var le = obj.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        le.preferredHeight = 40;
    }

    private void ShowCreateGroupPanel()
    {
        HideAllPanels();
        if (createGroupPanel != null) createGroupPanel.SetActive(true);
        if (codeDisplayPanel != null) codeDisplayPanel.SetActive(false);
        if (groupNameInput != null) groupNameInput.text = "";
        HighlightNav(navGroupsHighlight);
    }

    private void HideCreateGroupPanel()
    {
        if (createGroupPanel != null) createGroupPanel.SetActive(false);
        ShowHome();
    }

    private async void OnCreateGroup()
    {
        string name = groupNameInput?.text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            ToastManager.Show("Введите название группы");
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
            ToastManager.Show($"Группа «{name}» создана! Код: {group.code}");
            LoadGroups();
        }
    }

    private void ShowGroupsList()
    {
        if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
        ShowHome();
    }

    private void RefreshStudents()
    {
        if (!string.IsNullOrEmpty(selectedGroupId))
            _ = LoadStudents(selectedGroupId);
    }

    private void HideAllPanels()
    {
        if (homeView != null) homeView.SetActive(false);
        if (createGroupPanel != null) createGroupPanel.SetActive(false);
        if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
        if (codeDisplayPanel != null) codeDisplayPanel.SetActive(false);
        if (assignmentBuilderPanel != null) assignmentBuilderPanel.SetActive(false);
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

    private void ArchiveSelectedGroup()
    {
        if (string.IsNullOrEmpty(selectedGroupId)) return;
        var group = groups.Find(g => g.groupId == selectedGroupId);
        if (group == null) return;

        if (SessionManager.Instance != null && SessionManager.Instance.IsSelfStudy)
        {
            group.archived = !group.archived;
            if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
            ShowHome();
            LoadGroups();
            return;
        }
        _ = ArchiveGroupAsync(group);
    }

    private async System.Threading.Tasks.Task ArchiveGroupAsync(GroupData group)
    {
        bool newState = !group.archived;
        var data = new Dictionary<string, object> { { "archived", newState } };
        bool ok = await FirebaseManager.Instance.Service.UpdateDocument("groups", selectedGroupId, data);
        if (ok)
        {
            group.archived = newState;
            if (groupDetailPanel != null) groupDetailPanel.SetActive(false);
            DisplayGroups();
        }
    }

    private void ExportToCsv()
    {
        if (currentStudents.Count == 0)
        {
            ToastManager.Show("Нет данных для экспорта");
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
        ToastManager.Show($"Отчёт сохранён на рабочий стол");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void DownloadCsvWebGL(string csv, string filename);
#else
    private static void DownloadCsvWebGL(string csv, string filename) { }
#endif
}
