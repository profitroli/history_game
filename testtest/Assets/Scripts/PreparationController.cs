using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PreparationController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button lecturesButton;
    [SerializeField] private Button contentPage1Button; // ������ 1
    [SerializeField] private Button contentPage2Button; // ������ 2
    [SerializeField] private Button contentPage3Button; // ������ 3
    [SerializeField] private CanvasGroup contentGroup;

    [Header("��������� ����")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string lecturesScene = "LecturesScene";
    [SerializeField] private string contentPage1Scene = "ContentPage1"; // ����� 1
    [SerializeField] private string contentPage2Scene = "ContentPage2"; // ����� 2
    [SerializeField] private string contentPage3Scene = "ContentPage3"; // ����� 3
    [SerializeField] private float fadeTime = 0.8f;

    void Start()
    {
        // ��������� ����������� ������
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        if (lecturesButton != null)
        {
            lecturesButton.onClick.AddListener(OnLecturesButtonClick);
        }
        else
        {
            Debug.LogWarning("������ �������� � ������� �� ��������� � ����������!");
        }

        // ����������� ��� ������ ������� ��������
        if (contentPage1Button != null)
        {
            contentPage1Button.onClick.AddListener(() => OnContentPageButtonClick(contentPage1Scene, "�������� 1"));
        }
        else
        {
            Debug.LogWarning("������ ContentPage1 �� ��������� � ����������!");
        }

        if (contentPage2Button != null)
        {
            contentPage2Button.onClick.AddListener(() => OnContentPageButtonClick(contentPage2Scene, "�������� 2"));
        }
        else
        {
            Debug.LogWarning("������ ContentPage2 �� ��������� � ����������!");
        }

        if (contentPage3Button != null)
        {
            contentPage3Button.onClick.AddListener(() => OnContentPageButtonClick(contentPage3Scene, "�������� 3"));
        }
        else
        {
            Debug.LogWarning("������ ContentPage3 �� ��������� � ����������!");
        }

        ApplyRoleFilter();

        // ������ ��������� �������� (������ ��� ����� � �����)
        if (contentGroup != null)
        {
            contentGroup.alpha = 0;
            StartCoroutine(FadeInContent());
        }
    }

    private void ApplyRoleFilter()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;

        // ��������� ��������� ���
        var titleText = GetComponentInChildren<TMPro.TMP_Text>();
        if (titleText != null && titleText.text.Contains("���"))
        {
            titleText.text = ContentFilter.GetPreparationLabel(SessionManager.Instance.Role);
        }
    }

    void OnBackButtonClick()
    {
        Debug.Log("������� � ������� ����");
        SceneManager.LoadScene(mainMenuScene);
    }

    void OnLecturesButtonClick()
    {
        Debug.Log("������� � �������");
        SceneManager.LoadScene(lecturesScene);
    }

    void OnContentPageButtonClick(string sceneName, string pageName)
    {
        Debug.Log($"������� �� {pageName}");
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeInContent()
    {
        yield return new WaitForSeconds(0.3f);

        float timer = 0f;
        while (timer < fadeTime)
        {
            contentGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        contentGroup.alpha = 1;
    }

    // �����������: ����� � ������� ���������
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (contentGroup != null)
        {
            float timer = 0f;
            while (timer < fadeTime)
            {
                contentGroup.alpha = Mathf.Lerp(1, 0, timer / fadeTime);
                timer += Time.deltaTime;
                yield return null;
            }
            contentGroup.alpha = 0;
        }

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
}