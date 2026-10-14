using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CandidateCheck : MonoBehaviour
{
    [System.Serializable]
    public class CandidateProfile
    {
        public string name;                  // �������� ���
        public string age;                   // �������� �������
        [TextArea] public string otherInfo;  // ��������� ��������� (����� ��������, ����)
        public Sprite photo;
        public bool isEligible;
    }

    [Header("������ ����")]
    public CandidateProfile[] profiles;
    private int currentIndex = 0;
    private int score = 0;

    [Header("������ �� UI")]
    public RectTransform candidateCard;
    public Image candidatePhotoDisplay;
    public TextMeshProUGUI nameText;          // ������ �� ����� �����
    public TextMeshProUGUI ageText;           // ������ �� ����� ��������
    public TextMeshProUGUI bioText;           // ������ �� ����� ���������
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject finishPanel;
    public TextMeshProUGUI resultLabel;

    [Header("��������� ��������")]
    public float fallSpeed = 1200f;
    public float targetYPosition = 0f;
    private bool isAnimating = false;

    void Start()
    {
        if (finishPanel != null) finishPanel.SetActive(false);
        candidateCard.anchoredPosition = new Vector2(0, 800);
        ShowCandidate();
    }

    void Update()
    {
        if (isAnimating)
        {
            candidateCard.anchoredPosition = Vector2.MoveTowards(
                candidateCard.anchoredPosition,
                new Vector2(0, targetYPosition),
                fallSpeed * Time.deltaTime
            );

            if (candidateCard.anchoredPosition.y == targetYPosition) isAnimating = false;
        }
    }

    public void MakeDecision(bool playerApproved)
    {
        if (isAnimating || currentIndex >= profiles.Length) return;

        if (playerApproved == profiles[currentIndex].isEligible)
        {
            score += 10;
            StartCoroutine(ShowFeedback("�����!", Color.forestGreen));
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            score -= 5;
            StartCoroutine(ShowFeedback("������!", Color.red));
        }

        currentIndex++;
        StartCoroutine(HideAndShowNextCandidate());
    }

    IEnumerator HideAndShowNextCandidate()
    {
        // ������� ����
        float elapsed = 0f;
        Vector2 startPos = candidateCard.anchoredPosition;
        Vector2 endPos = new Vector2(0, -1200);
        while (elapsed < 0.2f)
        {
            candidateCard.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / 0.4f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ShowCandidate();
        yield return new WaitForSeconds(0.1f);

        // ������ ������
        candidateCard.anchoredPosition = new Vector2(0, 1200);
        isAnimating = true;
    }

    void ShowCandidate()
    {
        if (currentIndex < profiles.Length)
        {
            // ������������ ������ �� ������ ��������� �����
            nameText.text = profiles[currentIndex].name;
            ageText.text = profiles[currentIndex].age;
            bioText.text = profiles[currentIndex].otherInfo;

            if (profiles[currentIndex].photo != null)
                candidatePhotoDisplay.sprite = profiles[currentIndex].photo;

            scoreText.text = "����: " + score;
            isAnimating = true;
        }
        else
        {
            ShowFinalResult();
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void ShowFinalResult()
    {
        candidateCard.gameObject.SetActive(false);
        finishPanel.SetActive(true);
        string praise = (score >= 30) ? "�� ������� ������ ���������� � ���������!" : "���������� ��� ���!";
        resultLabel.text = $"�����!\n����: {score}\n{praise}";
    }

    IEnumerator ShowFeedback(string msg, Color col)
    {
        feedbackText.text = msg;
        feedbackText.color = col;
        yield return new WaitForSeconds(0.8f);
        feedbackText.text = "���������� ��� ����� ����� �����������";
        feedbackText.color = Color.black;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}