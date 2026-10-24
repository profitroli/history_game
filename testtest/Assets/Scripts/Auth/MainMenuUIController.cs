using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Кнопка профиля (аватар в углу)")]
    [SerializeField] private Button profileButton;
    [SerializeField] private Image profileButtonIcon;

    [Header("Панели")]
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private GameObject avatarPanel;
    [SerializeField] private GameObject assignmentPanel;

    [Header("Внутри профиля — кнопки")]
    [SerializeField] private Button changeAvatarButton;
    [SerializeField] private Button openAssignmentsButton;
    [SerializeField] private Button closeProfileButton;
    [SerializeField] private Button logoutButton;

    [Header("Закрытие панелей")]
    [SerializeField] private Button avatarCloseButton;
    [SerializeField] private Button assignmentCloseButton;

    [Header("Возврат учителя")]
    [SerializeField] private Button backToTeacherButton;

    [Header("Профиль — инфо")]
    [SerializeField] private TMP_Text profileNameText;
    [SerializeField] private TMP_Text profileEmailText;
    [SerializeField] private TMP_Text profileRoleText;
    [SerializeField] private TMP_Text profileGroupText;
    [SerializeField] private Image profileAvatarImage;

    void Start()
    {
        if (profileButton != null) profileButton.onClick.AddListener(OpenProfile);
        if (changeAvatarButton != null) changeAvatarButton.onClick.AddListener(OpenAvatarFromProfile);
        if (openAssignmentsButton != null) openAssignmentsButton.onClick.AddListener(OpenAssignmentsFromProfile);
        if (closeProfileButton != null) closeProfileButton.onClick.AddListener(CloseAll);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        if (avatarCloseButton != null) avatarCloseButton.onClick.AddListener(ReturnToProfile);
        if (assignmentCloseButton != null) assignmentCloseButton.onClick.AddListener(ReturnToProfile);

        if (backToTeacherButton != null)
        {
            bool isTeacher = SessionManager.Instance != null && SessionManager.Instance.IsTeacher;
            backToTeacherButton.gameObject.SetActive(isTeacher);
            if (isTeacher) backToTeacherButton.onClick.AddListener(BackToTeacherDashboard);
        }

        HideAll();
        UpdateProfileButtonIcon();
    }

    public void OpenProfile()
    {
        HideAll();
        if (profilePanel != null)
        {
            profilePanel.SetActive(true);
            UpdateProfileInfo();
        }
    }

    public void CloseAll()
    {
        HideAll();
    }

    public void OpenAvatarFromProfile()
    {
        HideAll();
        if (avatarPanel != null) avatarPanel.SetActive(true);
    }

    public void OpenAssignmentsFromProfile()
    {
        HideAll();
        if (assignmentPanel != null) assignmentPanel.SetActive(true);
    }

    public void ReturnToProfile()
    {
        HideAll();
        if (profilePanel != null)
        {
            profilePanel.SetActive(true);
            UpdateProfileInfo();
        }
        UpdateProfileButtonIcon();
    }

    private void HideAll()
    {
        if (profilePanel != null) profilePanel.SetActive(false);
        if (avatarPanel != null) avatarPanel.SetActive(false);
        if (assignmentPanel != null) assignmentPanel.SetActive(false);
    }

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

    private void UpdateProfileButtonIcon()
    {
        var sprite = AvatarService.GetSelectedSprite();
        if (sprite != null)
        {
            if (profileButtonIcon != null) { profileButtonIcon.sprite = sprite; profileButtonIcon.color = Color.white; }
            if (profileAvatarImage != null) { profileAvatarImage.sprite = sprite; profileAvatarImage.color = Color.white; }
        }
    }

    private async void LoadGroupName()
    {
        if (profileGroupText == null) return;
        string gid = SessionManager.Instance.GroupId;
        var doc = await FirebaseManager.Instance.Service.GetDocument("groups", gid);
        if (doc != null && doc.ContainsKey("name"))
            profileGroupText.text = "Группа: " + doc["name"];
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
