using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Виды контента (inline внутри ContentArea)")]
    [SerializeField] private GameObject homeView;
    [SerializeField] private GameObject profileView;
    [SerializeField] private GameObject avatarView;
    [SerializeField] private GameObject settingsView;
    [SerializeField] private GameObject assignmentView;

    [Header("Кнопка профиля (аватар в углу)")]
    [SerializeField] private Button profileButton;
    [SerializeField] private Image profileButtonIcon;

    [Header("Внутри профиля — кнопки")]
    [SerializeField] private Button changeAvatarButton;
    [SerializeField] private Button openAssignmentsButton;
    [SerializeField] private Button closeProfileButton;
    [SerializeField] private Button logoutButton;

    [Header("Закрытие вложенных панелей")]
    [SerializeField] private Button avatarCloseButton;
    [SerializeField] private Button assignmentCloseButton;
    [SerializeField] private Button settingsBackButton;

    [Header("Возврат учителя")]
    [SerializeField] private Button backToTeacherButton;

    [Header("Сайдбар")]
    [SerializeField] private TMP_Text welcomeText;
    [SerializeField] private TMP_Text sidebarNameText;
    [SerializeField] private TMP_Text sidebarRoleText;
    [SerializeField] private Image sidebarAvatarImage;
    [SerializeField] private Slider sidebarProgressBar;
    [SerializeField] private TMP_Text sidebarProgressText;
    [SerializeField] private TMP_Text sidebarLevelText;
    [SerializeField] private TMP_Text sidebarAchievementsText;
    [SerializeField] private Button navHomeButton;
    [SerializeField] private Button navProfileButton;
    [SerializeField] private Button navAchievementsButton;
    [SerializeField] private Button navSettingsButton;
    [SerializeField] private Button navExitButton;

    [Header("Подсветка навигации")]
    [SerializeField] private Image navHomeHighlight;
    [SerializeField] private Image navProfileHighlight;
    [SerializeField] private Image navAchievementsHighlight;
    [SerializeField] private Image navSettingsHighlight;

    [Header("Профиль — инфо")]
    [SerializeField] private TMP_Text profileNameText;
    [SerializeField] private TMP_Text profileEmailText;
    [SerializeField] private TMP_Text profileRoleText;
    [SerializeField] private TMP_Text profileGroupText;
    [SerializeField] private Image profileAvatarImage;

    [Header("Настройки — инфо")]
    [SerializeField] private TMP_Text settingsUserInfoText;
    [SerializeField] private Button settingsLogoutButton;
    [SerializeField] private Slider audioSlider;

    private static readonly Color NavActive = new Color(0.30f, 0.50f, 0.22f, 1f);
    private static readonly Color NavInactive = new Color(0f, 0f, 0f, 0f);

    private GameObject achievementsView;

    void Start()
    {
        if (profileButton != null) profileButton.onClick.AddListener(ShowProfile);
        if (changeAvatarButton != null) changeAvatarButton.onClick.AddListener(ShowAvatar);
        if (openAssignmentsButton != null) openAssignmentsButton.onClick.AddListener(ShowAssignments);
        if (closeProfileButton != null) closeProfileButton.onClick.AddListener(ShowHome);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        if (avatarCloseButton != null) avatarCloseButton.onClick.AddListener(ShowProfile);
        if (assignmentCloseButton != null) assignmentCloseButton.onClick.AddListener(ShowProfile);
        if (settingsBackButton != null) settingsBackButton.onClick.AddListener(ShowHome);
        if (settingsLogoutButton != null) settingsLogoutButton.onClick.AddListener(OnLogout);

        if (backToTeacherButton != null)
        {
            bool isTeacher = SessionManager.Instance != null && SessionManager.Instance.IsTeacher;
            backToTeacherButton.gameObject.SetActive(isTeacher);
            if (isTeacher) backToTeacherButton.onClick.AddListener(BackToTeacherDashboard);
        }

        if (navHomeButton != null) navHomeButton.onClick.AddListener(ShowHome);
        if (navProfileButton != null) navProfileButton.onClick.AddListener(ShowProfile);
        if (navAchievementsButton != null) navAchievementsButton.onClick.AddListener(ShowAchievements);
        if (navSettingsButton != null) navSettingsButton.onClick.AddListener(ShowSettings);
        if (navExitButton != null) navExitButton.onClick.AddListener(OnLogout);

        WireAudioSlider();
        ShowHome();
        UpdateSidebar();
        UpdateProfileButtonIcon();
    }

    private void WireAudioSlider()
    {
        if (audioSlider == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var all = canvas.GetComponentsInChildren<Slider>(true);
                foreach (var s in all)
                {
                    if (s.name.Contains("Audio") || s.name.Contains("Volume"))
                    { audioSlider = s; break; }
                }
            }
        }
        if (audioSlider != null)
        {
            if (AudioManager.Instance != null)
                audioSlider.value = AudioManager.Instance.musicVolume;
            audioSlider.onValueChanged.AddListener(OnAudioSliderChanged);
        }
    }

    private void OnAudioSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume(value);
    }

    private void ShowView(GameObject view)
    {
        if (homeView != null) homeView.SetActive(view == homeView);
        if (profileView != null) profileView.SetActive(view == profileView);
        if (avatarView != null) avatarView.SetActive(view == avatarView);
        if (settingsView != null) settingsView.SetActive(view == settingsView);
        if (assignmentView != null) assignmentView.SetActive(view == assignmentView);
        if (achievementsView != null) achievementsView.SetActive(view == achievementsView);
    }

    private void HighlightNav(Image active)
    {
        if (navHomeHighlight != null) navHomeHighlight.color = (active == navHomeHighlight) ? NavActive : NavInactive;
        if (navProfileHighlight != null) navProfileHighlight.color = (active == navProfileHighlight) ? NavActive : NavInactive;
        if (navAchievementsHighlight != null) navAchievementsHighlight.color = (active == navAchievementsHighlight) ? NavActive : NavInactive;
        if (navSettingsHighlight != null) navSettingsHighlight.color = (active == navSettingsHighlight) ? NavActive : NavInactive;
    }

    public void ShowHome()
    {
        ShowView(homeView);
        HighlightNav(navHomeHighlight);
    }

    public void ShowProfile()
    {
        ShowView(profileView);
        HighlightNav(navProfileHighlight);
        UpdateProfileInfo();
    }

    public void ShowAvatar()
    {
        ShowView(avatarView);
        HighlightNav(navProfileHighlight);
    }

    public void ShowSettings()
    {
        ShowView(settingsView);
        HighlightNav(navSettingsHighlight);
        UpdateSettingsInfo();
        if (audioSlider != null && AudioManager.Instance != null)
            audioSlider.value = AudioManager.Instance.musicVolume;
    }

    public void ShowAssignments()
    {
        ShowView(assignmentView);
        HighlightNav(navProfileHighlight);
    }

    public void ShowAchievements()
    {
        if (achievementsView == null)
            BuildAchievementsView();
        UpdateAchievementsContent();
        ShowView(achievementsView);
        HighlightNav(navAchievementsHighlight);
    }

    public void OpenProfile() => ShowProfile();
    public void ReturnToProfile() => ShowProfile();
    public void CloseAll() => ShowHome();

    private void UpdateProfileInfo()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        if (profileNameText != null) profileNameText.text = SessionManager.Instance.DisplayName;
        if (profileEmailText != null)
            profileEmailText.text = SessionManager.Instance.IsSelfStudy ? "" : SessionManager.Instance.Email;

        if (profileRoleText != null)
        {
            switch (SessionManager.Instance.Role)
            {
                case UserRole.Teacher: profileRoleText.text = "Преподаватель"; break;
                case UserRole.SchoolStudent: profileRoleText.text = "Школьник"; break;
                default: profileRoleText.text = "Студент"; break;
            }
        }

        if (profileGroupText != null)
        {
            if (SessionManager.Instance.IsSelfStudy)
                profileGroupText.text = "Самостоятельное изучение";
            else if (!string.IsNullOrEmpty(SessionManager.Instance.GroupId))
                LoadGroupName();
            else
                profileGroupText.text = "Нет группы";
        }

        if (logoutButton != null)
            logoutButton.gameObject.SetActive(!SessionManager.Instance.IsSelfStudy);

        UpdateProfileButtonIcon();
    }

    private void UpdateSettingsInfo()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        if (settingsUserInfoText != null)
        {
            if (SessionManager.Instance.IsSelfStudy)
                settingsUserInfoText.text = $"{SessionManager.Instance.DisplayName} (самостоятельно)";
            else
                settingsUserInfoText.text = $"{SessionManager.Instance.DisplayName} ({SessionManager.Instance.Email})";
        }
        if (settingsLogoutButton != null)
            settingsLogoutButton.gameObject.SetActive(!SessionManager.Instance.IsSelfStudy);
    }

    private void UpdateSidebar()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        string displayName = SessionManager.Instance.DisplayName;

        if (welcomeText != null)
            welcomeText.text = $"Добро пожаловать, {displayName}!";
        if (sidebarNameText != null)
            sidebarNameText.text = displayName;
        if (sidebarRoleText != null)
        {
            switch (SessionManager.Instance.Role)
            {
                case UserRole.SchoolStudent: sidebarRoleText.text = "Школьник"; break;
                case UserRole.Teacher: sidebarRoleText.text = "Преподаватель"; break;
                default: sidebarRoleText.text = "Студент"; break;
            }
        }

        var progress = ProgressService.Instance != null ? ProgressService.Instance.GetProgress() : null;
        int completed = progress != null ? progress.LecturesCompleted + progress.GamesCompleted + progress.TestsCompleted : 0;
        int total = 62;

        if (sidebarProgressBar != null)
        {
            sidebarProgressBar.value = total > 0 ? (float)completed / total : 0;
            if (sidebarProgressText != null)
                sidebarProgressText.text = $"Прогресс  {completed} / {total}";
        }

        if (sidebarLevelText != null)
        {
            int level = Mathf.Max(1, completed / 5 + 1);
            sidebarLevelText.text = $"Уровень {level}";
        }

        if (sidebarAchievementsText != null)
        {
            int achCount = progress != null ? progress.GamesCompleted : 0;
            sidebarAchievementsText.text = $"{achCount} / 20";
        }
    }

    private void UpdateProfileButtonIcon()
    {
        var sprite = AvatarService.GetSelectedSprite();
        if (sprite != null)
        {
            if (profileButtonIcon != null) { profileButtonIcon.sprite = sprite; profileButtonIcon.color = Color.white; }
            if (profileAvatarImage != null) { profileAvatarImage.sprite = sprite; profileAvatarImage.color = Color.white; }
            if (sidebarAvatarImage != null) { sidebarAvatarImage.sprite = sprite; sidebarAvatarImage.color = Color.white; }
        }
    }

    private async void LoadGroupName()
    {
        if (profileGroupText == null) return;
        if (SessionManager.Instance.IsSelfStudy) { profileGroupText.text = "Самостоятельное изучение"; return; }
        if (FirebaseManager.Instance == null || FirebaseManager.Instance.Service == null) { profileGroupText.text = "Группа не определена"; return; }
        string gid = SessionManager.Instance.GroupId;
        var doc = await FirebaseManager.Instance.Service.GetDocument("groups", gid);
        if (doc != null && doc.ContainsKey("name"))
            profileGroupText.text = "Группа: " + doc["name"];
    }

    private TMP_Text achProgressText;
    private Transform achAvatarGrid;

    private void BuildAchievementsView()
    {
        var parent = homeView != null ? homeView.transform.parent : null;
        if (parent == null) return;

        achievementsView = new GameObject("AchievementsView");
        achievementsView.transform.SetParent(parent, false);
        var rt = achievementsView.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var vlg = achievementsView.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 15, 15);
        vlg.spacing = 12;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var bg = achievementsView.AddComponent<Image>();
        bg.color = new Color(0.98f, 0.96f, 0.92f, 1f);

        AddAchLabel(achievementsView.transform, "ДОСТИЖЕНИЯ", 30, FontStyles.Bold, new Color(0.24f, 0.16f, 0.1f), 40);

        achProgressText = AddAchLabel(achievementsView.transform, "", 22, FontStyles.Normal, new Color(0.3f, 0.25f, 0.2f), 80);

        AddAchLabel(achievementsView.transform, "АВАТАРЫ", 26, FontStyles.Bold, new Color(0.24f, 0.16f, 0.1f), 35);

        var gridGO = new GameObject("AvatarGrid");
        gridGO.transform.SetParent(achievementsView.transform, false);
        gridGO.AddComponent<RectTransform>();
        var glg = gridGO.AddComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(140, 70);
        glg.spacing = new Vector2(10, 8);
        glg.childAlignment = TextAnchor.UpperLeft;
        gridGO.AddComponent<LayoutElement>().flexibleHeight = 1;
        achAvatarGrid = gridGO.transform;

        var backGO = new GameObject("BackBtn");
        backGO.transform.SetParent(achievementsView.transform, false);
        backGO.AddComponent<RectTransform>();
        backGO.AddComponent<Image>().color = new Color(0.3f, 0.5f, 0.22f, 1f);
        var backBtn = backGO.AddComponent<Button>();
        backGO.AddComponent<LayoutElement>().preferredHeight = 40;
        var backTxt = new GameObject("T").AddComponent<TextMeshProUGUI>();
        backTxt.transform.SetParent(backGO.transform, false);
        backTxt.text = "Назад";
        backTxt.fontSize = 22;
        backTxt.color = Color.white;
        backTxt.alignment = TextAlignmentOptions.Center;
        var brt = backTxt.GetComponent<RectTransform>();
        brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
        brt.offsetMin = brt.offsetMax = Vector2.zero;
        backBtn.onClick.AddListener(ShowHome);

        achievementsView.SetActive(false);
    }

    private TMP_Text AddAchLabel(Transform parent, string text, int size, FontStyles style, Color color, float height)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        go.AddComponent<LayoutElement>().preferredHeight = height;
        return tmp;
    }

    private void UpdateAchievementsContent()
    {
        var progress = ProgressService.Instance != null ? ProgressService.Instance.GetProgress() : null;
        int lec = progress != null ? progress.LecturesCompleted : 0;
        int games = progress != null ? progress.GamesCompleted : 0;
        int tests = progress != null ? progress.TestsCompleted : 0;
        float avg = progress != null ? progress.AverageTestScore : 0;
        int total = lec + games + tests;
        int maxTotal = 62;
        int level = Mathf.Max(1, total / 5 + 1);

        if (achProgressText != null)
        {
            achProgressText.text =
                $"Уровень: {level}\n" +
                $"Лекции: {lec} / 14\n" +
                $"Мини-игры: {games} / 34\n" +
                $"Тесты: {tests} / 14\n" +
                $"Средний балл: {avg:F0}%\n" +
                $"Общий прогресс: {total} / {maxTotal} ({(maxTotal > 0 ? total * 100 / maxTotal : 0)}%)";
        }

        if (achAvatarGrid != null)
        {
            foreach (Transform child in achAvatarGrid) Destroy(child.gameObject);
            float overall = maxTotal > 0 ? (float)total / maxTotal : 0f;
            for (int i = 0; i < AvatarConfig.TotalAvatars; i++)
            {
                bool unlocked = AvatarConfig.IsUnlocked(i, lec, games, tests, overall);
                var item = new GameObject($"Ach_{i}");
                item.transform.SetParent(achAvatarGrid, false);
                item.AddComponent<RectTransform>();
                var hlg = item.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 6;
                hlg.padding = new RectOffset(6, 6, 4, 4);
                hlg.childControlWidth = false;
                hlg.childControlHeight = true;
                hlg.childForceExpandWidth = false;
                item.AddComponent<Image>().color = unlocked
                    ? new Color(0.85f, 0.95f, 0.82f, 1f)
                    : new Color(0.88f, 0.86f, 0.84f, 1f);

                var iconGO = new GameObject("Icon");
                iconGO.transform.SetParent(item.transform, false);
                var iconImg = iconGO.AddComponent<Image>();
                var sprite = AvatarConfig.LoadSprite(i);
                if (sprite != null) { iconImg.sprite = sprite; iconImg.preserveAspect = true; }
                else iconImg.color = unlocked ? new Color(0.6f, 0.8f, 0.5f) : Color.gray;
                iconGO.AddComponent<LayoutElement>().preferredWidth = 50;

                var textGO = new GameObject("Text");
                textGO.transform.SetParent(item.transform, false);
                textGO.AddComponent<RectTransform>();
                var tmp = textGO.AddComponent<TextMeshProUGUI>();
                tmp.text = AvatarConfig.Names[i] + (unlocked ? " [Открыт]" : "") + "\n" +
                           "<size=16>" + AvatarConfig.Descriptions[i] + "</size>";
                tmp.fontSize = 19;
                tmp.color = unlocked ? new Color(0.15f, 0.4f, 0.1f) : new Color(0.45f, 0.42f, 0.4f);
                textGO.AddComponent<LayoutElement>().flexibleWidth = 1;
            }
        }
    }

    private void BackToTeacherDashboard()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TeacherDashboardScene");
    }

    private void OnLogout()
    {
        FirebaseManager.Instance?.Service?.SignOut();
        SessionManager.Instance?.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
