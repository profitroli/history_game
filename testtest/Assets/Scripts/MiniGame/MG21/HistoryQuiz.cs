using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HistoryQuiz : MonoBehaviour
{
    [System.Serializable]
    public class HistoryItem
    {
        public string question;     // ����� �������
        public bool isIndependence; // ���������� �����: true - �������������, false - �����������
    }

    public List<HistoryItem> questions; // ������ ���� �������
    public TextMeshProUGUI eventDisplay; // ������ �� ����� �� ��������
    public TextMeshProUGUI resultDisplay; // ������ �� ������ (�����/������)
    public GameObject finishPanel;       // ���� ������ (������)
    public TextMeshProUGUI resultText;   // �������� ����� �� ������

    public float speed = 250f;
    private int currentIndex = 0;

    void Start()
    {
        ShowNextQuestion();
    }
    void FinishGame()
    {
        ProgressService.Instance?.ReportGameCompleted(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);

        if (finishPanel != null)
        {
            finishPanel.SetActive(true); // �������� ���� ������
            resultText.text = $"�����! ������� ���������, ��������� �������!";
        }

    }
    // ��� ������� �������� ��� ������� ������
    public void SelectAnswer(bool userChoice)
    {
        if (currentIndex >= questions.Count) return;

        // ���������: ������ �� ����� ������ � ������� � ������
        if (userChoice == questions[currentIndex].isIndependence)
        {
            resultDisplay.text = "�����!";
            resultDisplay.color = Color.forestGreen;
            currentIndex++;
            Invoke("ShowNextQuestion", 0.5f); // �������� ��������� ������ ����� ����������
        }
        else
        {
            resultDisplay.text = "������! �������� ��� ���.";
            resultDisplay.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void ShowNextQuestion()
    {
        if (currentIndex < questions.Count)
        {
            eventDisplay.text = questions[currentIndex].question;
            resultDisplay.text = "� ������ ������� ��� ���������?";
            resultDisplay.color = Color.black;
        }
        else
        {
            FinishGame();
        }
    }
}