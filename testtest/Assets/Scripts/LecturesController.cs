using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LecturesController : MonoBehaviour
{
    [Header("������")]
    public Button backButton;
    public Button[] lectureButtons;
    public Button miniGamesButton; // ����� ������ ��� ����-���

    [Header("UI ��������")]
    public Text lectureTitle;
    public Text lectureContent;
    public GameObject contentPanel;

    void Start()
    {
        // ��������� ������ ��������
        if (backButton != null)
            backButton.onClick.AddListener(ReturnToMainMenu);

        // ��������� ������ ����-���
        if (miniGamesButton != null)
            miniGamesButton.onClick.AddListener(GoToMiniGames);

        // ��������� ������ ������
        for (int i = 0; i < lectureButtons.Length; i++)
        {
            int index = i; // ��������� ����� ��� ���������
            lectureButtons[i].onClick.AddListener(() => ShowLecture(index));
        }

        // �������� ������� ������ ��� ������
        if (contentPanel != null)
            contentPanel.SetActive(false);

        ApplyRoleFilter();
    }

    private void ApplyRoleFilter()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;

        string[] lectureNames = {
            "Lecture 1", "Lecture 2", "Lecture 3", "Lecture 4", "Lecture 5",
            "Lecture 6", "Lecture 7", "Lecture 8", "Lecture 9", "Lecture 10",
            "Lecture 11-12", "Lecture 13", "Lecture 14"
        };

        for (int i = 0; i < lectureButtons.Length && i < lectureNames.Length; i++)
        {
            bool visible = ContentFilter.IsLectureVisible(lectureNames[i], SessionManager.Instance.Role);
            lectureButtons[i].gameObject.SetActive(visible);
        }
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture1()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 1");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture2()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 2");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture3()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 3");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture4()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 4");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture5()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 5");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture6()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 6");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture7()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 7");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture8()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 8");
    }

    // ����� ��� �������� �� ����� ����������
    public void LoadLecture9()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 9");
    }

    public void LoadLecture10()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 10");
    }

    public void LoadLecture11()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 11-12");
    }

    public void LoadLecture13()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 13");
    }

    public void LoadLecture14()
    {
        // ���������, ��� ����� "PreparationScene" ��������� � Build Settings
        SceneManager.LoadScene("Lecture 14");
    }

    public void ReturnToMainMenu()
    {
        MainMenuController.LoadMainMenuFromOtherScene();
    }

    void GoToMiniGames()
    {
        // ������� � ����� ����-���
        // ���������, ��� � ��� ���� ����� � ������ "MiniGames" � Build Settings
        SceneManager.LoadScene("GamesScene");
    }

    void ShowLecture(int lectureIndex)
    {
        // ���������� ������� ������
        if (contentPanel != null)
            contentPanel.SetActive(true);

        // ����� ��������� ������� ������ �� �������
        // ������:
        string[] lectureTitles = {
            "�������� � ����������������",
            "������ ����������",
            "��������� ������",
            "��������-��������������� ����������������"
        };

        string[] lectureContents = {
            "���������������� - ��� ������� �������� ������������ ��������...",
            "�������� - ��� ������������������ ����� ��� ������� ������...",
            "��������� ������ ��������� ���������� �������������� � ������� ������...",
            "��� - ��� ��������� ����������������, ���������� �� ��������� ��������..."
        };

        if (lectureIndex >= 0 && lectureIndex < lectureTitles.Length)
        {
            if (lectureTitle != null)
                lectureTitle.text = lectureTitles[lectureIndex];

            if (lectureContent != null)
                lectureContent.text = lectureContents[lectureIndex];
        }
    }
}