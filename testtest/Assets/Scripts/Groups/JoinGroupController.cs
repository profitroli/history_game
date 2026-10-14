using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinGroupController : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text groupInfoText;

    void Start()
    {
        if (joinButton != null) joinButton.onClick.AddListener(OnJoin);
        if (cancelButton != null) cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private async void OnJoin()
    {
        string code = codeInput?.text?.Trim()?.ToUpper();
        if (string.IsNullOrEmpty(code) || code.Length < 6)
        {
            if (statusText != null) statusText.text = "Введите 6-значный код группы";
            return;
        }

        if (statusText != null) statusText.text = "Поиск группы...";

        var group = await GroupService.JoinGroup(code);

        if (group != null)
        {
            if (groupInfoText != null)
                groupInfoText.text = $"Группа: {group.name}\nПреподаватель: {group.teacherName}\nПрограмма: {(group.IsSchool ? "Школьная" : "Студенческая")}";
            if (statusText != null) statusText.text = "Вы присоединились к группе!";

            ProgressService.Instance?.LoadFromFirestore();
            Invoke(nameof(GoToMainMenu), 2f);
        }
        else
        {
            if (statusText != null) statusText.text = "Группа с таким кодом не найдена";
        }
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
