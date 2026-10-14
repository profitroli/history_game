using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfileController : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text emailText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text groupText;
    [SerializeField] private TMP_Text statsText;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button saveNameButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button joinGroupButton;
    [SerializeField] private GameObject joinGroupPanel;

    void Start()
    {
        if (saveNameButton != null) saveNameButton.onClick.AddListener(OnSaveName);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        if (joinGroupButton != null) joinGroupButton.onClick.AddListener(ShowJoinPanel);

        UpdateDisplay();
    }

    void OnEnable()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;

        if (nameText != null) nameText.text = SessionManager.Instance.DisplayName;
        if (emailText != null) emailText.text = SessionManager.Instance.Email;

        string roleStr;
        switch (SessionManager.Instance.Role)
        {
            case UserRole.Teacher: roleStr = "Преподаватель"; break;
            case UserRole.SchoolStudent: roleStr = "Школьник"; break;
            default: roleStr = "Студент"; break;
        }
        if (roleText != null) roleText.text = roleStr;

        if (groupText != null)
        {
            groupText.text = SessionManager.Instance.IsSelfStudy
                ? "Самостоятельное изучение"
                : $"Группа: {SessionManager.Instance.GroupId}";
        }

        if (statsText != null && ProgressService.Instance != null)
        {
            var p = ProgressService.Instance.GetProgress();
            statsText.text = $"Лекции: {p.LecturesCompleted} | Игры: {p.GamesCompleted} | Тесты: {p.TestsCompleted}";
        }

        if (joinGroupButton != null)
            joinGroupButton.gameObject.SetActive(SessionManager.Instance.IsSelfStudy || string.IsNullOrEmpty(SessionManager.Instance.GroupId));

        if (nameInput != null)
            nameInput.text = SessionManager.Instance.DisplayName;
    }

    private async void OnSaveName()
    {
        string newName = nameInput?.text?.Trim();
        if (string.IsNullOrEmpty(newName)) return;

        if (!SessionManager.Instance.IsSelfStudy)
        {
            var data = new System.Collections.Generic.Dictionary<string, object> { { "displayName", newName } };
            await FirebaseManager.Instance.Service.UpdateDocument("users", SessionManager.Instance.UserId, data);
        }

        SessionManager.Instance.Initialize(
            SessionManager.Instance.UserId,
            SessionManager.Instance.Email,
            newName,
            SessionManager.Instance.Role,
            SessionManager.Instance.GroupId);

        UpdateDisplay();
    }

    private void OnLogout()
    {
        FirebaseManager.Instance?.Service?.SignOut();
        SessionManager.Instance?.Clear();
        SceneManager.LoadScene("LoginScene");
    }

    private void ShowJoinPanel()
    {
        if (joinGroupPanel != null) joinGroupPanel.SetActive(true);
    }
}
