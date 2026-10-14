using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginController : MonoBehaviour
{
    [Header("Панели")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject roleSelectionPanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Вход")]
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;
    [SerializeField] private Button selfStudyButton;
    [SerializeField] private TMP_Text loginErrorText;

    [Header("Регистрация")]
    [SerializeField] private TMP_InputField regNameInput;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    [SerializeField] private TMP_InputField regGroupCodeInput;
    [SerializeField] private Toggle regTeacherToggle;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button backToLoginButton;
    [SerializeField] private TMP_Text regErrorText;

    [Header("Выбор роли (самостоятельно)")]
    [SerializeField] private TMP_InputField selfStudyNameInput;
    [SerializeField] private Button schoolStudentButton;
    [SerializeField] private Button universityStudentButton;
    [SerializeField] private Button backFromRoleButton;

    void Start()
    {
        EnsureServices();

        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClick);
        if (goToRegisterButton != null) goToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        if (selfStudyButton != null) selfStudyButton.onClick.AddListener(ShowRoleSelectionPanel);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClick);
        if (backToLoginButton != null) backToLoginButton.onClick.AddListener(ShowLoginPanel);
        if (schoolStudentButton != null) schoolStudentButton.onClick.AddListener(() => OnSelfStudyRole(UserRole.SchoolStudent));
        if (universityStudentButton != null) universityStudentButton.onClick.AddListener(() => OnSelfStudyRole(UserRole.UniversityStudent));
        if (backFromRoleButton != null) backFromRoleButton.onClick.AddListener(ShowLoginPanel);

        ShowLoginPanel();
        CheckInviteLink();
    }

    private void CheckInviteLink()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string url = Application.absoluteURL;
        if (!string.IsNullOrEmpty(url) && url.Contains("code="))
        {
            int idx = url.IndexOf("code=") + 5;
            string code = "";
            for (int i = idx; i < url.Length && i < idx + 6; i++)
            {
                char c = url[i];
                if (c == '&' || c == '#') break;
                code += c;
            }
            if (code.Length >= 6 && regGroupCodeInput != null)
            {
                regGroupCodeInput.text = code;
                ShowRegisterPanel();
                Debug.Log($"[Login] Инвайт-код из URL: {code}");
            }
        }
#endif
    }

    private void EnsureServices()
    {
        if (FirebaseManager.Instance == null)
        {
            var go = new GameObject("FirebaseManager");
            go.AddComponent<FirebaseManager>();
        }
        if (SessionManager.Instance == null)
        {
            var go = new GameObject("SessionManager");
            go.AddComponent<SessionManager>();
        }
        if (ProgressService.Instance == null)
        {
            var go = new GameObject("ProgressService");
            go.AddComponent<ProgressService>();
        }
    }

    private void ShowLoginPanel()
    {
        SetPanel(loginPanel);
        ClearErrors();
    }

    private void ShowRegisterPanel()
    {
        SetPanel(registerPanel);
        ClearErrors();
    }

    private void ShowRoleSelectionPanel()
    {
        SetPanel(roleSelectionPanel);
    }

    private void SetPanel(GameObject active)
    {
        if (loginPanel != null) loginPanel.SetActive(loginPanel == active);
        if (registerPanel != null) registerPanel.SetActive(registerPanel == active);
        if (roleSelectionPanel != null) roleSelectionPanel.SetActive(roleSelectionPanel == active);
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }

    private void ShowLoading(bool show)
    {
        if (loadingPanel != null) loadingPanel.SetActive(show);
    }

    private void ClearErrors()
    {
        if (loginErrorText != null) loginErrorText.text = "";
        if (regErrorText != null) regErrorText.text = "";
    }

    private async void OnLoginClick()
    {
        string email = loginEmailInput?.text?.Trim();
        string password = loginPasswordInput?.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError(loginErrorText, "Введите email и пароль");
            return;
        }

        ShowLoading(true);
        var result = await FirebaseManager.Instance.Service.SignInWithEmail(email, password);
        ShowLoading(false);

        if (!result.success)
        {
            ShowError(loginErrorText, result.error);
            return;
        }

        await LoadUserAndRoute(result.userId, result.email);
    }

    private async void OnRegisterClick()
    {
        string name = regNameInput?.text?.Trim();
        string email = regEmailInput?.text?.Trim();
        string password = regPasswordInput?.text;
        string groupCode = regGroupCodeInput?.text?.Trim();
        bool isTeacher = regTeacherToggle != null && regTeacherToggle.isOn;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError(regErrorText, "Заполните все поля");
            return;
        }
        if (password.Length < 6)
        {
            ShowError(regErrorText, "Пароль должен быть не менее 6 символов");
            return;
        }

        ShowLoading(true);
        var result = await FirebaseManager.Instance.Service.SignUpWithEmail(email, password);

        if (!result.success)
        {
            ShowLoading(false);
            ShowError(regErrorText, result.error);
            return;
        }

        UserRole role = isTeacher ? UserRole.Teacher : UserRole.UniversityStudent;
        string groupId = "";

        if (!isTeacher && !string.IsNullOrEmpty(groupCode))
        {
            var groups = await FirebaseManager.Instance.Service.QueryByField("groups", "code", groupCode);
            if (groups != null && groups.Count > 0)
            {
                var group = groups[0];
                groupId = group.ContainsKey("__docId__") ? group["__docId__"].ToString() : "";
                string program = group.ContainsKey("program") ? group["program"].ToString() : "university";
                role = program == "school" ? UserRole.SchoolStudent : UserRole.UniversityStudent;
            }
            else
            {
                ShowLoading(false);
                ShowError(regErrorText, "Группа с таким кодом не найдена");
                return;
            }
        }

        var userData = new Dictionary<string, object>
        {
            { "email", email },
            { "displayName", name },
            { "role", UserData.RoleToString(role) },
            { "groupId", groupId },
            { "createdAt", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };
        await FirebaseManager.Instance.Service.SetDocument("users", result.userId, userData);

        ShowLoading(false);

        SessionManager.Instance.Initialize(result.userId, email, name, role, groupId);
        ProgressService.Instance.LoadFromFirestore();
        RouteByRole(role);
    }

    private void OnSelfStudyRole(UserRole role)
    {
        string name = selfStudyNameInput?.text?.Trim();
        if (string.IsNullOrEmpty(name)) name = "Гость";

        SessionManager.Instance.InitializeSelfStudy(role, name);
        RouteByRole(role);
    }

    private async System.Threading.Tasks.Task LoadUserAndRoute(string userId, string email)
    {
        var doc = await FirebaseManager.Instance.Service.GetDocument("users", userId);
        if (doc == null)
        {
            ShowError(loginErrorText, "Профиль не найден. Зарегистрируйтесь.");
            return;
        }

        string name = doc.ContainsKey("displayName") ? doc["displayName"].ToString() : "Пользователь";
        string roleStr = doc.ContainsKey("role") ? doc["role"].ToString() : "university_student";
        string groupId = doc.ContainsKey("groupId") ? doc["groupId"].ToString() : "";

        UserRole role;
        switch (roleStr)
        {
            case "teacher": role = UserRole.Teacher; break;
            case "school_student": role = UserRole.SchoolStudent; break;
            default: role = UserRole.UniversityStudent; break;
        }

        SessionManager.Instance.Initialize(userId, email, name, role, groupId);
        ProgressService.Instance.LoadFromFirestore();
        RouteByRole(role);
    }

    private void RouteByRole(UserRole role)
    {
        switch (role)
        {
            case UserRole.Teacher:
                SceneManager.LoadScene("TeacherDashboardScene");
                break;
            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    private void ShowError(TMP_Text errorText, string message)
    {
        if (errorText != null)
            errorText.text = message;
        Debug.LogWarning($"[Login] {message}");
    }
}
