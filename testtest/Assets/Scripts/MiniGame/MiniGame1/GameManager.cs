using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DropZone[] zones;
    public TextMeshProUGUI statusText;
    public int totalCountries; // ����� ����� ����� ���������� ����� � ����
    public string originalStatusText;

    void Start()
    {
        // ���������� �����, ������� �� �������� � Unity ����� ��������
        if (statusText != null)
        {
            originalStatusText = statusText.text;
        }
    }
    public void CheckResult()
    {
        int placedItemsCount = 0;
        bool allGood = true;

        // 1. �������, ������� ����� �������� �������� � ����
        foreach (var zone in zones)
        {
            DragItem[] itemsInZone = zone.GetComponentsInChildren<DragItem>();
            placedItemsCount += itemsInZone.Length;

            // ��������� ������������
            foreach (var item in itemsInZone)
            {
                if (item.type != zone.zoneType) allGood = false;
            }
        }

        // 2. ��������� �������
        if (placedItemsCount == 0)
        {
            statusText.text = "������� ����������!";
            statusText.color = Color.purple;
        }
        else if (placedItemsCount < totalCountries)
        {
            statusText.text = "�������� ��� ��������!";
            statusText.color = Color.purple;
        }
        else
        {
            // ���� �� ���������, ���������� ���������
            statusText.text = allGood ? "�������!!" : "�� ���������, ������� ���";
            statusText.color = allGood ? Color.forestGreen : Color.red;
            if (allGood)
                ProgressService.Instance?.ReportGameCompleted(SceneManager.GetActiveScene().name, 1);
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ������� ��� ������ "�����"
    public void ResetGame()
    {
        // ������� ������ ��� �������� � ����� � �������� �� ����� ������
        DragItem[] allCards = FindObjectsOfType<DragItem>();
        foreach (var card in allCards)
        {
            card.ResetCard();
        }
        statusText.text = originalStatusText;
        statusText.color = Color.black;// ������� �����
    }
}