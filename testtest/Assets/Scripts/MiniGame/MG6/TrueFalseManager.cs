using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrueFalseManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public ToggleAnswer toggleBtn; // ������ �� ������-�������������
        public bool isActuallyTrue;    // ���������� ����� �� �������
    }

    public List<QuestionData> questions;
    public TextMeshProUGUI statusText;
    public string originalStatusText;

    void Start()
    {
        // ���������� �����, ������� �� �������� � Unity ����� ��������
        if (statusText != null)
        {
            originalStatusText = statusText.text;
        }
    }

    public void CheckAllAnswers()
    {
        int correctCount = 0;
        bool allAnswered = true;

        foreach (var q in questions)
        {
            if (q.toggleBtn.currentState == ToggleAnswer.State.None) allAnswered = false;
        }

        if (!allAnswered)
        {
            statusText.text = "�������� �� ��� �������!";
            return;
        }

        foreach (var q in questions)
        {
            bool userSaidTrue = (q.toggleBtn.currentState == ToggleAnswer.State.True);

            if (userSaidTrue == q.isActuallyTrue)
            {
                correctCount++;
                q.toggleBtn.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            }
            else
            {
                q.toggleBtn.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
        }

        statusText.text = (correctCount == questions.Count) ? "�����������!" : $"�����: {correctCount} �� {questions.Count}";
        statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var q in questions) q.toggleBtn.ResetButton();
        statusText.text = originalStatusText;
        statusText.color = Color.black;
    }
}