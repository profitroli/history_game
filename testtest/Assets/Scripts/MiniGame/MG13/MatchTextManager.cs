using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchTextManage : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    private MatchTextItem selectedDate;
    private Dictionary<MatchTextItem, MatchTextItem> currentMatches = new Dictionary<MatchTextItem, MatchTextItem>();

    void Start()
    {
        statusText.text = "�������� ���� �����";
        statusText.color = Color.black;
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void SelectDate(MatchTextItem dateItem)
    {
        // ���������� ������ ���������
        if (selectedDate != null && !currentMatches.ContainsKey(selectedDate))
            selectedDate.SetColor(Color.black);

        selectedDate = dateItem;
        selectedDate.SetColor(Color.blue);
        statusText.text = "�������: " + dateItem.GetComponent<TextMeshProUGUI>().text + ". ������ �������� �������.";
    }

    public void SelectEvent(MatchTextItem eventItem)
    {
        if (selectedDate == null) return;

        // --- ������ ����������� ������ ������� ---
        float targetY = selectedDate.transform.localPosition.y;
        MatchTextItem occupant = null;

        // ����, ��� ������ �������� ��� ������ (�� Y)
        MatchTextItem[] allItems = FindObjectsByType<MatchTextItem>(FindObjectsSortMode.None);
        foreach (var item in allItems)
        {
            if (!item.isLeftColumn && item != eventItem && Mathf.Abs(item.transform.localPosition.y - targetY) < 5f)
            {
                occupant = item;
                break;
            }
        }

        // ���� ����� ������, ���������� "���������" ����, ��� ������ ����� ��������� �������
        if (occupant != null)
        {
            Vector3 tempPos = occupant.transform.localPosition;
            occupant.transform.localPosition = new Vector3(occupant.originalPosition.x, eventItem.transform.localPosition.y, 0);
        }

        // ���������� ��������� ������� �� ������ � ����
        eventItem.transform.localPosition = new Vector3(eventItem.originalPosition.x, targetY, 0);

        // ��������� ����� � �������
        if (currentMatches.ContainsKey(selectedDate)) currentMatches.Remove(selectedDate);

        // ���� ��� ������� ��� ���� ��������� � ������ ����, ����������� �� ����
        MatchTextItem oldDate = null;
        foreach (var pair in currentMatches) { if (pair.Value == eventItem) oldDate = pair.Key; }
        if (oldDate != null) { currentMatches.Remove(oldDate); oldDate.SetColor(Color.black); }

        currentMatches[selectedDate] = eventItem;

        selectedDate.SetColor(Color.purple);
        eventItem.SetColor(Color.purple);

        selectedDate = null;
        statusText.text = "���� ����������!";
    }

    public void CheckAll()
    {
        int correctCount = 0;
        int total = FindObjectsByType<MatchTextItem>(FindObjectsSortMode.None).Length / 2;

        if (currentMatches.Count < total)
        {
            statusText.text = "��������� ��� ���� ����� ���������!";
            statusText.color = Color.red;
            return;
        }

        foreach (var pair in currentMatches)
        {
            if (pair.Key.id == pair.Value.id)
            {
                pair.Key.SetColor(Color.forestGreen);
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
                pair.Value.SetColor(Color.forestGreen);
                correctCount++;
            }
            else
            {
                pair.Key.SetColor(Color.red);
                pair.Value.SetColor(Color.red);
            }
        }

        if (correctCount == total)
        {
            statusText.text = "�����! �������� ������.";
            statusText.color = Color.forestGreen;
        }
        else
        {
            statusText.text = "������! ��������� ������� ����.";
            statusText.color = Color.red;
        }
    }

    public void ResetGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}