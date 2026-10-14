using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SourceGameManager : MonoBehaviour
{
    [System.Serializable]
    public class SourceItem
    {
        public TMP_InputField inputField;
        public string correctAnswer;
    }

    public SourceItem[] sources;
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
    public void CheckAnswers()
    {
        int correct = 0;
        int filledCount = 0;

        foreach (var item in sources)
        {
            // ���������� Trim(), ����� ������ ��������� �������, ������� ������ ��������
            string currentText = item.inputField.text.Trim();

            if (!string.IsNullOrEmpty(currentText))
            {
                filledCount++;
            }
        }

        if (filledCount == 0)
        {
            statusText.text = "�� ������ �� ��������!";
            return;
        }

        if (filledCount < sources.Length)
        {
            statusText.text = "��������� ��� ��������!";
            statusText.color = Color.purple;
            return; // ���������� ��������
        }

        // 3. ���� ��� ���� ���������, ��������� ������������
        foreach (var item in sources)
        {
            if (item.inputField.text.Trim().ToLower() == item.correctAnswer.ToLower())
            {
                correct++;
                item.inputField.image.color = Color.green;
            }
            else
            {
                item.inputField.image.color = Color.red;
            }
        }

        if (correct == sources.Length)
        {
            statusText.text = "��� �����! �� �������� �������!";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "���� ������. ��������� ������� ����.";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var item in sources)
        {
            item.inputField.text = "";
            item.inputField.image.color = Color.white;
        }
        statusText.text = originalStatusText;
        statusText.color = Color.black;
    }
}