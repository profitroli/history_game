using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour
{
    [System.Serializable]
    public class TimelineEvent
    {
        public DraggableCard cardScript; // ����� ���� ���� ��������
        public int correctSlotID;       // ���������� ID ����� (0, 1, 2...)
    }

    public List<TimelineEvent> events;
    public TextMeshProUGUI statusText;

    public void CheckResults()
    {
        int correctCount = 0;

        foreach (var ev in events)
        {
            if (ev.cardScript.currentSlotID == ev.correctSlotID)
            {
                correctCount++;
            }
        }

        if (correctCount == events.Count)
        {
            statusText.text = "�����! ���������� ���������.";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "������ � �������! ���������� ��� ���.";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        if (statusText != null) statusText.text = "���������� ������� � ��������������� �������";
        statusText.color= Color.black;

        foreach (var ev in events)
        {
            if (ev.cardScript != null)
            {
                ev.cardScript.ResetPosition();
            }
        }
    }
}