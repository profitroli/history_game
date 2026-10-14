using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiDateManager : MonoBehaviour
{
    [System.Serializable]
    public class DateEntry
    {
        public string eventName;      // �������� ��� ���������� (��������, "��������� ������������")
        public TMP_InputField inputField; // ����, ���� ����� ����� ���
        public string correctYear;    // ���������� ��� (��������, "1863")
        public Image iconImage;       // �������� ������� (����� ������ � ���� ��� ��������)
    }

    public List<DateEntry> dateEntries;
    public TextMeshProUGUI statusText;

    public void CheckAllDates()
    {
        int correctCount = 0;

        foreach (var entry in dateEntries)
        {
            // ������� ���� �� ������ �������� � ���� (��������� ������ �����)
            string userYear = System.Text.RegularExpressions.Regex.Replace(entry.inputField.text, @"\D", "");

            if (userYear == entry.correctYear)
            {
                correctCount++;
                // ������������ ���� �������
                entry.inputField.image.color = Color.green;
            }
            else
            {
                // ������������ ���� �������
                entry.inputField.image.color = Color.red;
            }
        }

        // ����� ��������� ����� ������
        if (correctCount == dateEntries.Count)
        {
            statusText.text = "�������! ��� ���� ������� �����.";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = $"�����: {correctCount} �� {dateEntries.Count}. ��������� ������!";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetDates()
    {
        foreach (var entry in dateEntries)
        {
            entry.inputField.text = "";
            entry.inputField.image.color = Color.white;
        }
        statusText.text = "������� ���� �������";
        statusText.color = Color.black;
    }
}