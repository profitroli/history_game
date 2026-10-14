using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LawMarathon : MonoBehaviour
{
    [System.Serializable]
    public class LawFact
    {
        public string text;
        public bool isTrue;
    }

    public LawFact[] facts;
    public RectTransform card;
    public TextMeshProUGUI factText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText; // ����� "�������!" ��� "�� ��..."
    public GameObject finishPanel;       // ���� ������ (������)
    public TextMeshProUGUI resultText;   // �������� ����� �� ������

    public float speed = 250f;
    private int currentIndex = -1;
    private int score = 0;
    private bool isMoving = true;

    void Start()
    {
        ; // ������ ����� � ������
        if (feedbackText != null) feedbackText.text = "���������� ������ ������ �����������";      // ������� ���������
        PrepareNextFact();
    }

    void Update()
    {
        if (!isMoving || currentIndex >= facts.Length) return;

        card.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        if (card.anchoredPosition.x < -800f)
        {
            if (facts[currentIndex].isTrue)
            {
                score -= 5;
                ShowFeedback("��������� ������! ���� ������������.", Color.black);
            }
            PrepareNextFact();
        }
    }

    public void AcceptFact()
    {
        if (!isMoving) return;

        if (Mathf.Abs(card.anchoredPosition.x) < 150f)
        {
            if (facts[currentIndex].isTrue)
            {
                score += 10;
                ShowFeedback("�������! ��������� ������.", Color.forestGreen);
            }
            else
            {
                score -= 10;
                ShowFeedback("�� ��! � ��������� ��� ������ ���������.", Color.red);
            }
            PrepareNextFact();
        }
    }

    // ����� ��� ����������� �������� ���������
    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            // ������������� ������ ��������, ���� ��� ���, � ��������� �����
            StopAllCoroutines();
            StartCoroutine(ClearFeedback());
        }
    }

    IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(3f);
        feedbackText.text = "";
    }

    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ResetGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    void PrepareNextFact()
    {
        currentIndex++;
        if (currentIndex < facts.Length)
        {
            factText.text = facts[currentIndex].text;
            card.anchoredPosition = new Vector2(800f, -35.31f);
            scoreText.text = "����: " + score;
        }
        else
        {
            FinishGame();
        }
    }

    // ����� ��������� ����
    void FinishGame()
    {
        ProgressService.Instance?.ReportGameCompleted(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, score);
        isMoving = false;
        card.gameObject.SetActive(false);

        if (finishPanel != null)
        {
            finishPanel.SetActive(true); // �������� ���� ������

            // ��������� ������� � ����������� �� �����
            string praise = score >= 35 ? "�� ��������� ������� � �����!" : "������� ���������, ��������� �������!";
            resultText.text = $"�����!\n���� ����: {score} �����.\n{praise}";
        }
        else
        {
            // ���� ������ ���, ������� ������ � ����� ��������
            factText.text = $"�����! ����: {score}";
        }
    }
}