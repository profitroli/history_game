using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Навигация")]
    [SerializeField] private Button backButton;

    [Header("Аккаунт (опционально)")]
    [SerializeField] private Button logoutButton;
    [SerializeField] private TMP_Text userInfoText;

    void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(LoadMainMenu);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        UpdateUserInfo();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnLogout()
    {
        FirebaseManager.Instance?.Service?.SignOut();
        SessionManager.Instance?.Clear();
        SceneManager.LoadScene("LoginScene");
    }

    private void UpdateUserInfo()
    {
        if (userInfoText == null || SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;

        if (SessionManager.Instance.IsSelfStudy)
            userInfoText.text = $"{SessionManager.Instance.DisplayName} (самостоятельно)";
        else
            userInfoText.text = $"{SessionManager.Instance.DisplayName} ({SessionManager.Instance.Email})";

        if (logoutButton != null)
            logoutButton.gameObject.SetActive(!SessionManager.Instance.IsSelfStudy);
    }
}