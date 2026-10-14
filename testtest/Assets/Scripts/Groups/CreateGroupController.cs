using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateGroupController : MonoBehaviour
{
    [SerializeField] private TMP_InputField groupNameInput;
    [SerializeField] private Toggle schoolToggle;
    [SerializeField] private Toggle universityToggle;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private GameObject resultPanel;

    void Start()
    {
        if (createButton != null) createButton.onClick.AddListener(OnCreate);
        if (cancelButton != null) cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private async void OnCreate()
    {
        string name = groupNameInput?.text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            if (resultText != null) resultText.text = "Введите название группы";
            return;
        }

        string program = (schoolToggle != null && schoolToggle.isOn) ? "school" : "university";
        var group = await GroupService.CreateGroup(name, program);

        if (group != null)
        {
            if (codeText != null) codeText.text = group.code;
            if (resultText != null) resultText.text = $"Группа «{group.name}» создана!";
            if (resultPanel != null) resultPanel.SetActive(true);
        }
        else
        {
            if (resultText != null) resultText.text = "Ошибка создания группы";
        }
    }
}
