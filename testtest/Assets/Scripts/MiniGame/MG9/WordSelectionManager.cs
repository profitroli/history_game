using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class WordElement
    {
        public string itemName;      // �������� (��������, "������")
        public Button wordButton;    // ������ � UI
        public bool isCorrectTarget; // ������ �� ��� ����� ���� �������?
        [HideInInspector] public bool isSelected = false;
    }

    public List<WordElement> words;
    public TextMeshProUGUI statusText;

    void Start()
    {
        foreach (var w in words)
        {
            WordElement current = w;
            current.wordButton.onClick.AddListener(() => OnWordClicked(current));
            current.wordButton.image.color = Color.white;
        }
        statusText.text = "�������� ��, ��� ����� ����� ��� ��� � ������:";
    }

    void OnWordClicked(WordElement w)
    {
        w.isSelected = !w.isSelected;
        // ��������� ��� ������ (������� ����)
        w.wordButton.image.color = w.isSelected ? new Color(0.6f, 0.8f, 1f) : Color.white;
    }

    public void CheckResults()
    {
        int correctSelected = 0;
        int totalRequired = 0;
        bool hasErrors = false;

        foreach (var w in words)
        {
            if (w.isCorrectTarget) totalRequired++;

            if (w.isSelected)
            {
                if (w.isCorrectTarget)
                {
                    // ��������� ��������� �������
                    w.wordButton.image.color = Color.green;
                    correctSelected++;
                }
                else
                {
                    // �������� ��������� �������
                    w.wordButton.image.color = Color.red;
                    hasErrors = true;
                }
            }
            else
            {
                // ���� ������ �� �������, ���������� �� ����� ���� (��� �����)
                w.wordButton.image.color = Color.white;

                // ���� ����� ����� ������� ������ ������
                if (w.isCorrectTarget) hasErrors = true;
            }
        }

        if (!hasErrors && correctSelected == totalRequired)
        {
            statusText.text = "�����! �� ������ ����������� � ����.";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "���� ������ ��� ����������� ��������!";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var w in words)
        {
            w.isSelected = false;
            w.wordButton.image.color = Color.white;
        }
        statusText.text = "�������� ��, ��� ����� ����� ��� ��� � ������";
        statusText.color = Color.black;
    }
}