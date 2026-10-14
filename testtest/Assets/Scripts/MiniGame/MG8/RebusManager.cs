using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RebusManager : MonoBehaviour
{
    [System.Serializable]
    public class RebusQuestion
    {
        public string rebusName;      // ������ ��� ������� � ����������
        public TMP_InputField inputField; // ���� ����� ��� ����� ������
        public Image resultImage;     // ��������-�����, ������� �������� (���� �����)
        public Image backImage;
        public string correctText;    // ���������� �����
    }

    public List<RebusQuestion> questions; // ������ ���� 3 �������
    public TextMeshProUGUI statusText;    // ����� ����� ����������

    void Start()
    {
        // ��� ������ �������� ��������-������, ���� ��� ���������
        foreach (var q in questions)
        {
            if (q.resultImage != null) q.resultImage.gameObject.SetActive(false);
        }
    }

    public void CheckAllAnswers()
    {
        int correctCount = 0;

        foreach (var q in questions)
        {
            // 1. ������� ������� �� �����, ������ ���������� �������
            // 2. .Replace(" ", "") � ������� ��� ������� ������ (����� "��� �����" ����� ��� ����)
            string userText = q.inputField.text.Trim().ToLower().Replace(" ", "");
            string targetText = q.correctText.Trim().ToLower().Replace(" ", "");

            // ���������, ��� ���� �� ������ � ������ ������� ��� ����� ��������
            if (userText == targetText && targetText != "")
            {
                correctCount++;
                q.inputField.image.color = Color.green; // ������� � �����
                if (q.resultImage != null) { q.resultImage.gameObject.SetActive(true); q.backImage.gameObject.SetActive(false); }
            }
            else
            {
                q.inputField.image.color = Color.red; // ������� � ������
            }
        }

        if (correctCount == questions.Count)
        {
            statusText.text = "��� ������ ���������!"; statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }else
        {
            statusText.text = $"�����: {correctCount} �� {questions.Count}"; statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var q in questions)
        {
            q.inputField.text = "";
            q.inputField.image.color = Color.white;
            if (q.resultImage != null) { q.resultImage.gameObject.SetActive(false); q.backImage.gameObject.SetActive(true); }
            }
        statusText.text = "���������� ������ �� ������� ���";
        statusText.color = Color.black;
    }
}